import { v4 as uuid } from 'https://jspm.dev/uuid';

class Theme {
  static glaucousHEX = '#7180acff';
  static marianBlueHEX = '#2b4570ff';
  static lightBlueHEX = '#a8d0dbff';
  static atomicTangerineHEX = '#e49273ff';
  static cinereousHEX = '#a37a74ff';

  static glaucousHSLA = 'hsla(225, 26%, 56%, 1)';
  static marianBlueHSLA = 'hsla(217, 45%, 30%, 1)';
  static lightBlueHSLA = 'hsla(193, 41%, 76%, 1)';
  static atomicTangerineHSLA = 'hsla(16, 68%, 67%, 1)';
  static cinereousHSLA = 'hsla(8, 20%, 55%, 1)';

  static glaucousRGBA = 'rgba(113, 128, 172, 1)';
  static marianBlueRGBA = 'rgba(43, 69, 112, 1)';
  static lightBlueRGBA = 'rgba(168, 208, 219, 1)';
  static atomicTangerineRGBA = 'rgba(228, 146, 115, 1)';
  static cinereousRGBA = 'rgba(163, 122, 116, 1)';

  static gradientTop = 'linear-gradient(0deg, #7180acff, #2b4570ff, #a8d0dbff, #e49273ff, #a37a74ff)';
  static gradientRight = 'linear-gradient(90deg, #7180acff, #2b4570ff, #a8d0dbff, #e49273ff, #a37a74ff)';
  static gradientBottom = 'linear-gradient(180deg, #7180acff, #2b4570ff, #a8d0dbff, #e49273ff, #a37a74ff)';
  static gradientLeft = 'linear-gradient(270deg, #7180acff, #2b4570ff, #a8d0dbff, #e49273ff, #a37a74ff)';
  static gradientTopRight = 'linear-gradient(45deg, #7180acff, #2b4570ff, #a8d0dbff, #e49273ff, #a37a74ff)';
  static gradientBottomRight = 'linear-gradient(135deg, #7180acff, #2b4570ff, #a8d0dbff, #e49273ff, #a37a74ff)';
  static gradientTopLeft = 'linear-gradient(225deg, #7180acff, #2b4570ff, #a8d0dbff, #e49273ff, #a37a74ff)';
  static gradientBottomLeft = 'linear-gradient(315deg, #7180acff, #2b4570ff, #a8d0dbff, #e49273ff, #a37a74ff)';
  static gradientRadial = 'radial-gradient(#7180acff, #2b4570ff, #a8d0dbff, #e49273ff, #a37a74ff)';
}

class Widget {
  constructor(id, description, type, grabPosition, size, content, backgroundColor, borderColor, location) {
    this.id = id;
    this.description = description;
    this.type = type;
    this.grabPosition = grabPosition;
    this.size = size;
    this.content = content;
    this.backgroundColor = backgroundColor;
    this.borderColor = borderColor;
    this.position = location;
  }
}
window.Widget = Widget;


// SignalR setup
var connection = new signalR.HubConnectionBuilder().withUrl("/webprojects").build();
let saveButton = document.getElementById("save-btn");
saveButton.disabled = true;

connection.start().then(() => {
  saveButton.disabled = false;
}).catch((err) => {
  return console.error(err.toString());
});


let urlParams = new URLSearchParams(window.location.search);
let projectId = urlParams.get("projectId");
let canvas = document.getElementById("canvas");


// JS Helper functions
export function createElementFromWidget(widgetData, event) {
  let newElement = document.createElement("div");
  let canvasRect = document.getElementById("canvas").getBoundingClientRect();
  let newPositionX = event.clientX - canvasRect.left - widgetData.grabPosition.x;
  let newPositionY = event.clientY - canvasRect.top - widgetData.grabPosition.y;
  widgetData.position = { "x": newPositionX, "y": newPositionY };

  newElement.style.width = "150px";
  newElement.style.height = "70px";
  newElement.style.borderRadius = "4px";
  newElement.style.backgroundColor = widgetData.backgroundColor;
  newElement.style.borderColor = widgetData.borderColor;
  newElement.style.display = "flex";
  newElement.style.justifyContent = "center";
  newElement.style.alignItems = "center";
  newElement.style.cursor = "grab";
  newElement.style.resize = "both";
  newElement.style.overflow = "auto";
  newElement.draggable = true;

  newElement.dataset.widgetType = widgetData.type;
  newElement.innerHTML = widgetData.content;
  newElement.id = `${widgetData.type}_${widgetData.id}`;
  newElement.style.position = "absolute";
  newElement.style.top = `${newPositionY}px`;
  newElement.style.left = `${newPositionX}px`;

  newElement.dataset.isSelected = false;
  newElement.addEventListener("click", (e) => {
    if (newElement.dataset.isSelected == "true") {
      newElement.blur();
      newElement.classList.remove("selected-widget");
      newElement.dataset.isSelected = false;
    } else {
      newElement.focus();
      newElement.classList.add("selected-widget");
      newElement.dataset.isSelected = true;
    }
  });

  return newElement;
}

export function handleDroppedWidgetDragStart(event, elementId) {
  event.stopPropagation();
  let elementRect = event.target.getBoundingClientRect();
  let grabPosition = {
    "id": elementId,
    "x": event.clientX - elementRect.left,
    "y": event.clientY - elementRect.top
  };
  let json = JSON.stringify(grabPosition);

  event.dataTransfer.effectAllowed = "move";
  event.dataTransfer.dropEffect = "move";
  event.dataTransfer.setData("grabPosition", json);
}

export function handleWidgetDragOver(e) {
  e.preventDefault();
  return false;
}

export function handleCanvasDragOver(e) {
  e.preventDefault();
  return false;
}

export function handleCanvasDrop(e) {
  e.stopPropagation();

  if (e.dataTransfer.effectAllowed === "copy") {
    let widgetData = JSON.parse(e.dataTransfer.getData("widgetData"));

    let newElement = createElementFromWidget(widgetData, e);

    newElement.addEventListener("dragstart", (e) => handleDroppedWidgetDragStart(e, newElement.id));
    //resizeObserver.observe(newElement);

    if (connection != null) {
      connection.invoke("AddWidgetToWebProject", projectId, widgetData).catch((err) => {
        return console.error(err.toString());
      });
    }
    e.target.appendChild(newElement);
  }
  else {
    let json = e.dataTransfer.getData("grabPosition");
    if (json != null && json.length > 0) {
      let grabPosition = JSON.parse(json);

      let canvasRect = document.getElementById("canvas").getBoundingClientRect();
      let newPositionX = e.clientX - canvasRect.left - grabPosition.x;
      let newPositionY = e.clientY - canvasRect.top - grabPosition.y;

      let draggingElement = document.getElementById(grabPosition.id);
      draggingElement.style.setProperty("top", `${newPositionY}px`);
      draggingElement.style.setProperty("left", `${newPositionX}px`);

      if (connection != null) {
        let widgetGuid = grabPosition.id.split("_").pop();
        connection.invoke("UpdateWidgetPosition", widgetGuid, newPositionX, newPositionY).catch((err) => {
          return console.error(err.toString());
        });
      }
    }
    else {
      console.error("no drag data received");
    }
  }

  return false;
}


document.querySelectorAll(".widget-preview").forEach(element => {
  element.addEventListener("dragstart", (event) => {
    let widgetType = element.dataset.widgetType;
    let styles = element.computedStyleMap();

    let elementRect = event.target.getBoundingClientRect();
    let widget = new Widget(
      uuid(),
      null,
      widgetType,
      { "x": event.clientX - elementRect.left, "y": event.clientY - elementRect.top },
      { "x": 150, "y": 70 },
      element.innerHTML,
      styles.get("background-color").toString(),
      styles.get("border-color").toString(),
      null
    );

    event.dataTransfer.effectAllowed = "copy";
    event.dataTransfer.setData("widgetData", JSON.stringify(widget));
  });
  element.addEventListener("dragover", handleWidgetDragOver);
});

document.querySelectorAll(".dropped-widget").forEach(element => {
  element.addEventListener("dragstart", (e) => handleDroppedWidgetDragStart(e, element.id));
})

document.addEventListener("keydown", (event) => {
  if (event.code === "Delete") {
    let currentWidgets = [...canvas.children];
    let currentlyFocusedWidgets = currentWidgets.filter(w => w.getAttribute("data-is-selected") === "true");

    if (connection != null) {
      currentlyFocusedWidgets.forEach(w => {
        let widgetGuid = w.id.split("_").pop();
        connection.invoke("RemoveWidgetFromWebProject", projectId, widgetGuid).catch((err) => {
          return console.error(err.toString());
        });
        w.remove();
      });
    }
    else {
      alert("Internal error: Could not remove widget. Please try refreshing the page.");
    }
  }
});

canvas.addEventListener("dragover", handleCanvasDragOver);
canvas.addEventListener("drop", handleCanvasDrop);
canvas.addEventListener("click", (e) => {
  if (e.target.id === canvas.id) {
    let currentWidgets = [...canvas.children];
    let currentlyFocusedWidgets = currentWidgets.filter(w => w.getAttribute("data-is-selected") === "true");
    currentlyFocusedWidgets.forEach(w => {
      w.setAttribute("data-is-selected", "false");
      w.classList.remove("selected-widget");
    });
  }
});

document.getElementById("clear-canvas-btn").addEventListener("click", (e) => {
  if (connection != null) {
    connection.invoke("RemoveAllWidgetsFromWebProject", projectId).catch(err => {
      console.error(err.toString());
    });
    canvas.replaceChildren();
  }
});