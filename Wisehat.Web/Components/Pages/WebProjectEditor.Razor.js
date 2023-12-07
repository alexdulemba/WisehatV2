// Author: Alexander Dulemba
// Copyright 2023

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
// Following Microsoft tutorial: https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-8.0&tabs=visual-studio
const connection = new signalR.HubConnectionBuilder().withUrl("/webprojects").build();
connection.onclose(error => alert("Editor connection closed. Try refreshing the page."));

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
let saveIndicator = document.getElementById("save-indicator");

let contextMenu = document.getElementById("context-menu");
let fillColorInput = document.getElementById("widget-fill-color-input");
let borderColorInput = document.getElementById("widget-border-color-input");
let borderThicknessInput = document.getElementById("widget-border-thickness-input");
let widgetDeleteButton = document.getElementById("widget-delete-btn");
let changeWidgetContentButton = document.getElementById("widget-change-content-btn");
let changeWidgetContentModal = document.getElementById("edit-widget-content-modal-backdrop");
let widgetContentModalTitle = document.getElementById("modal-title");
let widgetContentModalInput = document.getElementById("modal-input");
let saveContentChangesButton = document.getElementById("save-content-changes-btn");
let cancelContentChangesButton = document.getElementById("cancel-content-changes-btn");

window.onbeforeunload = (e) => {
  connection.invoke("RemoveAllWidgetsFromWebProject", projectId).catch(error => {
    console.error(error.toString());
  });
};

// Use ResizeObserver API to listen for widget element resizes: https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserver
let resizeObserverEntries = {};
const resizeObserver = new ResizeObserver(entries => {
  entries.forEach(entry => {
    let timeoutId = resizeObserverEntries[entry.target.id];
    clearTimeout(timeoutId);
    resizeObserverEntries[entry.target.id] = setTimeout(() => {
      saveIndicator.classList.remove("hidden");
      if (connection != null) {
        connection.invoke("UpdateWidgetSize", parseWidgetGuid(entry.target.id), entry.contentRect.width, entry.contentRect.height).catch(err => {
          console.error(err.toString());
        });
      }
      else {
        console.error("SignalR connection not initialized.");
      }
    }, 500);
  });
});


// Custom JS Helper functions

// this method is dervied from StackOverflow: https://stackoverflow.com/questions/1740700/how-to-get-hex-color-value-rather-than-rgb-value
function convertRgbToHex(rgb) {
  if (rgb.startsWith("#")) return rgb;

  let hexString = rgb.trim()
    .substring(4, rgb.length - 1)
    .split(", ")
    .map(n => parseInt(n).toString(16))
    .join("");

  return "#" + hexString;
}

function handleShowContextMenu(e, element) {
  e.preventDefault();
  if (contextMenu.dataset.hidden === "true") {
    contextMenu.style.top = `${e.clientY}px`;
    contextMenu.style.left = `${e.clientX}px`;
    contextMenu.classList.remove("hidden");
    contextMenu.dataset.hidden = "false";
    contextMenu.dataset.targetWidgetId = e.target.id;

    fillColorInput.value = convertRgbToHex(element.style.backgroundColor);
    borderColorInput.value = convertRgbToHex(element.style.borderColor);
    borderThicknessInput.value = element.style.borderWidth.slice(0, -2);
  }
  else {
    contextMenu.classList.add("hidden");
    contextMenu.dataset.hidden = "true";
    contextMenu.dataset.targetWidgetId = "";
  }
}

function parseWidgetGuid(widgetId) {
  return widgetId.split("_").pop();
}

function handleWidgetFocus(event, element) {
  if (element.dataset.isSelected == "true") {
    element.blur();
    element.classList.remove("selected-widget");
    element.dataset.isSelected = false;
  }
  else {
    element.focus();
    element.classList.add("selected-widget");
    element.dataset.isSelected = true;
  }
}

function createElementFromWidget(widgetData, event) {
  let newElement = document.createElement("div");
  let canvasRect = document.getElementById("canvas").getBoundingClientRect();
  let newPositionX = event.clientX - canvasRect.left - widgetData.grabPosition.x;
  let newPositionY = event.clientY - canvasRect.top - widgetData.grabPosition.y;
  widgetData.position = { "x": newPositionX, "y": newPositionY };

  newElement.style.width = "150px";
  newElement.style.height = "70px";
  newElement.style.borderRadius = "4px";
  newElement.style.backgroundColor = Theme.atomicTangerineHEX;
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
  newElement.addEventListener("click", (e) => handleWidgetFocus(e, newElement));

  return newElement;
}

// calculations for mouse grab position derived from here: https://stackoverflow.com/questions/3234256/find-mouse-position-relative-to-element
// dragging code dervied from MDN: https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/drag_event
// and web.dev: https://web.dev/articles/drag-and-drop
function handleWidgetDragStart(event, element) {
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
}

function handleDroppedWidgetDragStart(event, elementId) {
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

// Derived from web.dev: https://web.dev/articles/drag-and-drop
function handleWidgetDragOver(e) {
  e.preventDefault();
  return false;
}

// Derived from web.dev: https://web.dev/articles/drag-and-drop
function handleCanvasDragOver(e) {
  e.preventDefault();
  return false;
}

// Derived from web.dev: https://web.dev/articles/drag-and-drop
function handleCanvasDrop(e) {
  e.stopPropagation();

  if (e.dataTransfer.effectAllowed === "copy") {
    let widgetData = JSON.parse(e.dataTransfer.getData("widgetData"));

    let newElement = createElementFromWidget(widgetData, e);

    newElement.addEventListener("dragstart", (e) => handleDroppedWidgetDragStart(e, newElement.id));
    newElement.addEventListener("contextmenu", (e) => handleShowContextMenu(e, newElement));
    resizeObserver.observe(newElement);

    if (connection != null) {
      connection.invoke("AddWidgetToWebProject", projectId, widgetData).catch((err) => {
        return console.error(err.toString());
      });
    }
    e.target.appendChild(newElement);
    saveIndicator.classList.remove("hidden");
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
        let widgetGuid = parseWidgetGuid(grabPosition.id);
        connection.invoke("UpdateWidgetPosition", widgetGuid, newPositionX, newPositionY).catch((err) => {
          return console.error(err.toString());
        });
      }
      saveIndicator.classList.remove("hidden");
    }
    else {
      console.error("no drag data received");
    }
  }

  return false;
}


export function initializeEventListeners() {
  console.log("initializing event listeners");

  document.querySelectorAll(".widget-preview").forEach(widget => {
    widget.addEventListener("dragstart", (e) => handleWidgetDragStart(e, widget));
    widget.addEventListener("dragover", handleWidgetDragOver);
  });

  document.querySelectorAll(".dropped-widget").forEach(widget => {
    widget.addEventListener("dragstart", (e) => handleDroppedWidgetDragStart(e, widget.id));
    resizeObserver.observe(widget);
    widget.addEventListener("contextmenu", (e) => handleShowContextMenu(e, widget));
    widget.addEventListener("click", (e) => handleWidgetFocus(e, widget));
  });

  canvas.addEventListener("dragover", handleCanvasDragOver);
  canvas.addEventListener("drop", handleCanvasDrop);
  canvas.addEventListener("click", (e) => {
    if (e.target.id === canvas.id) {
      //unfocus focused widget
      let currentWidgets = [...canvas.children];
      let currentlyFocusedWidgets = currentWidgets.filter(w => w.getAttribute("data-is-selected") === "true");
      currentlyFocusedWidgets.forEach(w => {
        w.setAttribute("data-is-selected", "false");
        w.classList.remove("selected-widget");
      });

      // hide context menu if showing
      contextMenu.classList.add("hidden");
      contextMenu.dataset.hidden = "true";
    }
  });

  document.getElementById("clear-canvas-btn").addEventListener("click", (e) => {
    if (connection != null) {
      connection.invoke("RemoveAllWidgetsFromWebProject", projectId).catch(err => {
        console.error(err.toString());
      });
      canvas.replaceChildren();

      saveIndicator.classList.remove("hidden");
    } else {
      console.error("SignalR connection not initialized");
    }
  });

  document.getElementById("save-btn").addEventListener("click", (e) => {
    if (connection != null) {
      connection.invoke("SaveWebProjectAsync", projectId).then((value) => {
        saveIndicator.classList.add("hidden");
      }).catch(err => {
        console.error(err.toString());
      });
    }
  });

  fillColorInput.addEventListener("change", (e) => {
    let targetWidgetId = contextMenu.dataset.targetWidgetId;
    if (targetWidgetId != null && targetWidgetId.length > 0) {
      let targetWidget = document.getElementById(targetWidgetId);
      targetWidget.style.backgroundColor = e.target.value;

      saveIndicator.classList.remove("hidden");

      if (connection != null) {
        let widgetGuid = parseWidgetGuid(targetWidgetId);
        connection.invoke("UpdateWidgetFillColor", widgetGuid, e.target.value).catch(err => {
          console.error(`Couldn't update widget fill color: ${err.toString()}`);
        });
      }
    }
  }, false);

  borderColorInput.addEventListener("change", (e) => {
    let targetWidgetId = contextMenu.dataset.targetWidgetId;
    if (targetWidgetId != null && targetWidgetId.length > 0) {
      let targetWidget = document.getElementById(targetWidgetId);
      targetWidget.style.border = `${borderThicknessInput.value}px solid ${e.target.value}`;

      saveIndicator.classList.remove("hidden");

      if (connection != null) {
        let widgetGuid = parseWidgetGuid(targetWidgetId);
        connection.invoke("UpdateWidgetBorder", widgetGuid, `${borderThicknessInput.value}px solid ${e.target.value}`).catch(err => {
          console.error(`Couldn't update widget border color: ${err.toString()}`);
        });
      }
    }
  }, false);

  borderThicknessInput.addEventListener("change", (e) => {
    let targetWidgetId = contextMenu.dataset.targetWidgetId;
    if (targetWidgetId != null && targetWidgetId.length > 0) {
      let targetWidget = document.getElementById(targetWidgetId);
      targetWidget.style.border = `${e.target.value}px solid ${borderColorInput.value}`;

      saveIndicator.classList.remove("hidden");

      if (connection != null) {
        let widgetGuid = parseWidgetGuid(targetWidgetId);
        connection.invoke("UpdateWidgetBorder", widgetGuid, `${e.target.value}px solid ${borderColorInput.value}`).catch(err => {
          console.error(`Couldn't update widget border thickness: ${err.toString()}`);
        });
      }
    }
  }, false);

  widgetDeleteButton.addEventListener("click", (e) => {
    let targetWidgetId = contextMenu.dataset.targetWidgetId;
    if (targetWidgetId != null && targetWidgetId.length > 0) {
      let targetWidget = document.getElementById(targetWidgetId);

      if (connection != null) {
        let widgetGuid = parseWidgetGuid(targetWidgetId);
        connection.invoke("RemoveWidgetFromWebProject", projectId, widgetGuid).catch((err) => {
          console.error(`Unable to delete widget: ${err.toString()}`);
        });
        targetWidget.remove();
        contextMenu.dataset.targetWidgetId = "";
        contextMenu.classList.add("hidden");
        saveIndicator.classList.remove("hidden");
      }
      else {
        alert("Internal error: Could not remove widget. Please try refreshing the page.");
      }
    }
  });

  changeWidgetContentButton.addEventListener("click", (e) => {
    let targetWidgetId = contextMenu.dataset.targetWidgetId;
    let targetWidgetType = targetWidgetId.split("_")[0].toLowerCase();

    if (targetWidgetType === "fillbox") {
      // do nothing
    }
    else if (targetWidgetType === "textbox") {
      widgetContentModalTitle.innerText = "Edit Textbox"
      widgetContentModalInput.value = document.getElementById(targetWidgetId).innerText;
      changeWidgetContentModal.classList.remove("hidden");
    }
    else if (targetWidgetType === "imagebox") {
      // open the file upload dialog
    }
    else if (targetWidgetType === "videobox") {
      widgetContentModalTitle.innerText = "Edit Videobox";
      widgetContentModalInput.setAttribute("placeholder", "Insert Link");
      changeWidgetContentModal.classList.remove("hidden");
    }
    else {
      console.error("Invalid widget type");
    }
  });

  saveContentChangesButton.addEventListener("click", (e) => {
    let newValue = widgetContentModalInput.value;
    let targetWidgetId = contextMenu.dataset.targetWidgetId;
    let targetWidgetType = targetWidgetId.split("_")[0].toLowerCase();

    if (targetWidgetType === "textbox") {
      document.getElementById(targetWidgetId).innerText = newValue;
      if (connection != null) {
        connection.invoke("UpdateWidgetContent", parseWidgetGuid(targetWidgetId), newValue).catch(err => {
          console.error(err.ToString());
        });
      }
    }
    else if (targetWidgetType === "imagebox") {
      // open the file upload dialog
    }
    else if (targetWidgetType === "videobox") {
      document.getElementById(targetWidgetId).children[0].setAttribute("src", newValue);
      let updatedInnerElement = document.getElementById(targetWidgetId).children[0].outerHTML;
      if (connection != null) {
        connection.invoke("UpdateWidgetContent", parseWidgetGuid(targetWidgetId), updatedInnerElement).catch(err => {
          console.error(err.ToString());
        });
      }
    }
    else {
      console.error("Invalid widget type");
    }

    changeWidgetContentModal.classList.add("hidden");
  });

  cancelContentChangesButton.addEventListener("click", (e) => {
    changeWidgetContentModal.classList.add("hidden");
  });

  saveIndicator.classList.add("hidden");
}