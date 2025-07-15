const { animate, createAnimatable, utils, eases, stagger } = anime;

function connect() {
  console.log("Connecting...");
  const ws = new WebSocket("ws://localhost:" + globalConfig.webSocketPort);
  ws.onopen = onOpen;
  ws.onclose = onClose;
  // ws.onerror = onError;
  ws.onmessage = onMessage;
  return ws;
}

let scoreMultiplier = 10;
let encryptionType;
let encryptionCount;

function onOpen() {
  console.log("Connected");
  document.getElementById("status").style.display = "none";
}

function onClose() {
  console.log("Disconnected");
  document.getElementById("status").style.display = "initial";
  showOverlay();

  setTimeout(connect, 10000);
}

function onMessage(e) {
  const event = JSON.parse(e.data);

  switch (event.type) {
    case "TNHLevelEvent":
      scoreMultiplier = event.status.scoreMultiplier;
      break;

    case "TNHEncryptionDestroyed":
      document.getElementById("encryption-count").textContent =
        "x" + --encryptionCount;
      animate("#encryption-count", {
        scale: [1.5, 1],
        duration: 250,
      });
      break;

    case "TNHHoldPhaseEvent":
      if (event.status.phase == "Analyzing") {
        encryptionType = event.status.encryption;
        encryptionCount = event.status.encryptionCount;

        document.getElementById("encryption-count").textContent =
          "x" + encryptionCount;
        document.getElementById("encryption-icon").src =
          `icons/${encryptionType}.webp`;

        document.getElementById("score").textContent = "00000";
        document.getElementById("score-lost").textContent = "-000";
        document.getElementById("clock-value").textContent = "0.0";

        showOverlay();
        setTimeout(() => {
          timerBar.setValue(1);
          timerBar.setColor("#fc0");
        }, 500);
      } else if (event.status.phase == "Hacking") {
        timerBar.setColor("#f80");
        countdown.start();
      } else if (event.status.phase == "Transition") {
        countdown.stop();
        timerBar.setColor("#0dd");
      }
      break;

    case "TNHPhaseEvent":
      if (event.status.phase == "Take" || event.status.phase == "Completed") {
        hideOverlay();
        timerBar.setValue(0);
      } else if (event.status.phase == "Dead") {
        countdown.stop();
        timerBar.setColor("#f44");
      }
      break;
  }
}

function showOverlay() {
  animate("#overlay", {
    maxWidth: 400,
    duration: 500,
    ease: eases.inOutCirc,
  });

  animate("#timer", {
    scale: 1,
    opacity: 1,
    duration: 750,
    delay: 250,
    ease: eases.outCubic,
  });

  animate("#counter > *", {
    x: 0,
    opacity: 1,
    delay: stagger(100, { start: 250 }),
    duration: 500,
    ease: eases.outCubic,
  });
}

function hideOverlay() {
  animate("#timer", {
    scale: 0.8,
    opacity: 0,
    duration: 750,
    ease: eases.outCubic,
  });

  animate("#counter > *", {
    x: -16,
    opacity: 0,
    delay: stagger(100),
    duration: 500,
    ease: eases.outCubic,
  });

  animate("#overlay", {
    maxWidth: 0,
    duration: 500,
    delay: 500,
    ease: eases.inOutCirc,
  });
}

class Countdown {
  constructor() {
    this.interval = null;
    this.duration = 120;
    this.value = this.duration;
  }

  start() {
    this.value = this.duration;
    if (this.interval != null) return;

    this.interval = setInterval(() => {
      this.value = Math.max(0, this.value - 0.1);

      const progress = this.value / this.duration;
      const clock = (this.duration - this.value).toFixed(1);
      const score = Math.floor(this.value * 50 * scoreMultiplier);
      const scoreLost = -(6000 * scoreMultiplier - score);

      if (this.value <= 60) timerBar.setColor("#f44");
      timerBar.setValue((progress % 0.5) * 2);

      document.getElementById("score").textContent = score;
      document.getElementById("score-lost").textContent = scoreLost;
      document.getElementById("clock-value").textContent = clock;
    }, 100);
  }

  stop() {
    clearInterval(this.interval);
    this.interval = null;
  }
}

class Canvas {
  constructor(id) {
    this.element = document.getElementById(id);
    this.width = this.element.getBoundingClientRect().width;
    this.height = this.element.getBoundingClientRect().height;
    this.element.width = this.width * window.devicePixelRatio;
    this.element.height = this.height * window.devicePixelRatio;
    this.ctx = this.element.getContext("2d");
    this.ctx.scale(window.devicePixelRatio, window.devicePixelRatio);
    this.ctx.translate(0.5, 0.5);
  }

  clear() {
    this.ctx.clearRect(0, 0, this.width, this.height);
  }
}

class CircleBar extends Canvas {
  constructor(id, start, end) {
    super(id);

    this.start = start;
    this.end = end;

    this.value = 0;
    this.color = "#fc0";
    this.valueAnimator = createAnimatable(this, {
      value: 500,
      onUpdate: () => this.draw(this.value),
    });

    this.segments = 4;
    this.draw(this.value);
  }

  setValue(value) {
    this.valueAnimator.value(value);
  }

  setColor(color) {
    animate(this, {
      color: color,
      duration: 500,
      onUpdate: () => this.draw(this.value),
    });
  }

  draw(value) {
    this.clear();
    const distance = this.end - this.start;
    const angle = this.start + distance * value;

    this.ctx.shadowBlur = 8;

    this.ctx.beginPath();
    this.ctx.globalAlpha = 0.1;
    this.ctx.lineWidth = 20;
    this.ctx.strokeStyle = "#fff";
    this.ctx.shadowColor = "transparent";
    this.ctx.arc(this.width / 2, this.height / 2, 70, this.start, this.end);
    this.ctx.stroke();

    this.ctx.beginPath();
    this.ctx.globalAlpha = 1;
    this.ctx.shadowColor = this.color;
    this.ctx.strokeStyle = this.color;
    this.ctx.arc(this.width / 2, this.height / 2, 70, this.start, angle);
    this.ctx.stroke();

    this.ctx.beginPath();
    this.ctx.globalAlpha = 0.25;
    this.ctx.lineWidth = 10;
    this.ctx.strokeStyle = "#fff";
    this.ctx.shadowColor = "transparent";
    this.ctx.arc(this.width / 2, this.height / 2, 65, this.start, this.end);
    this.ctx.stroke();

    this.ctx.beginPath();
    this.ctx.arc(
      this.width / 2,
      this.height / 2,
      65,
      this.end + 0.1,
      this.start - 0.1,
    );
    this.ctx.stroke();
  }
}

const countdown = new Countdown();
const timerBar = new CircleBar("timer-bar", Math.PI, 2.5 * Math.PI);
connect();
