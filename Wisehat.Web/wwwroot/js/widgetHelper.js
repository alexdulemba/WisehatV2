import { Widget } from './widget.js';

export class Theme {
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