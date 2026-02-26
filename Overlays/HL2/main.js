function connect() {
  console.log("Connecting...");
  const ws = new WebSocket("ws://localhost:" + globalConfig.webSocketPort);
  ws.onopen = onOpen;
  ws.onclose = onClose;
  ws.onmessage = onMessage;
  return ws;
}

function onOpen(e) {
  document.getElementById("status").style.opacity = 0;
}

function onClose(e) {
  document.getElementById("status").style.opacity = 1;
  setTimeout(() => {
    connect();
  }, 10000);
}

function onMessage(e) {
  const event = JSON.parse(e.data);

  switch (event.type || event.event) {
    case "hello":
      console.log(event);
      return;
    case "sceneEvent":
      handleSceneEvent(event.status);
      return;
    case "ammoEvent":
      handleAmmoEvent(event.status);
      break;
    case "healthEvent":
      handleHealthEvent(event.status);
      break;
    case "TNHTokenEvent":
      handleTokenEvent(event.status);
      break;
    default:
      // console.log(event);
      break;
  }
}

HTMLElement.prototype.setText = function (text) {
  if (this.textContent == text) return;

  this.textContent = text;
  this.setAttribute("data-text", text);

  requestAnimationFrame(() => {
    this.classList.add("glowing");
    requestAnimationFrame(() => {
      this.classList.remove("glowing");
    });
  });
};

const ammo = document.getElementById("ammo");
const ammoIcon = document.getElementById("ammo-icon");
const ammoCap = document.getElementById("ammo-cap");
const ammoPanel = document.getElementById("ammo-panel");
function handleAmmoEvent(event) {
  ammo.setText(event.current);
  ammoCap.setText(event.capacity);

  event.current == 0
    ? ammoPanel.classList.add("red")
    : ammoPanel.classList.remove("red");

  switch (event.weapon) {
    default:
    case "USPMatch":
    case "USPMatch9mm":
      ammoIcon.textContent = "p";
      break;
    case "Python":
      ammoIcon.textContent = "q";
      break;
    case "Mp9":
      ammoIcon.textContent = "r";
      break;
    case "SPAS 12":
    case "SPAS 12 Tactical":
      ammoIcon.textContent = "s";
      break;
    case "Sustenance AR3":
      ammoIcon.textContent = "u";
      break;
    case "Sustenance Crossbow":
      ammoIcon.textContent = "w";
      break;
  }
}

const health = document.getElementById("health");
const healthPanel = document.getElementById("health-panel");
function handleHealthEvent(event) {
  const percent = event.health / event.maxHealth;
  health.setText(Math.max(0, Math.ceil(percent * 100)));

  percent <= 0.2
    ? healthPanel.classList.add("warning")
    : healthPanel.classList.remove("warning");
}

const aux = document.getElementById("aux-fg");
const auxPanel = document.getElementById("aux");
function handleTokenEvent(event) {
  aux.style.width = event.tokens * 10 + "%";
  auxPanel.style.opacity = event.tokens == 0 ? 0 : 1;
}

function handleSceneEvent(event) {
  ammo.setText(0);
  ammoCap.setText(0);
  ammoPanel.classList.remove("red");
}

connect();
