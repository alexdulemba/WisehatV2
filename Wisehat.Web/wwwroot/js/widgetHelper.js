import { Widget } from './widget.js';

//> .widget-preview {
//  width: 88%;
//  height: 50%;
//  border-radius: 4px;
//  background-color: $atomic-tangerine;
//  display: flex;
//  justify-content: center;
//  align-items: center;

//  &:hover {
//    cursor: grab;
//  }
//}

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


export function createElementFromWidget(widget, event, onDragStart) {
  let newElement = document.createElement("div");
  let canvasRect = document.getElementById("canvas").getBoundingClientRect();
  let newPositionX = event.clientX - canvasRect.left - widget.grabPosition[0];
  let newPositionY = event.clientY - canvasRect.top - widget.grabPosition[1];
  widget.location = [newPositionX, newPositionY];
  newElement.dataset.relativeLocation = widget.location;

  newElement.style.width = "150px";
  newElement.style.height = "70px";
  newElement.style.borderRadius = "4px";
  newElement.style.backgroundColor = Theme.atomicTangerineHEX;
  newElement.style.display = "flex";
  newElement.style.justifyContent = "center";
  newElement.style.alignItems = "center";
  newElement.style.cursor = "grab";
  newElement.style.resize = "both";
  newElement.draggable = true;

  newElement.id = `${widget.widgetType}_${widget.id}`;
  newElement.style.position = "absolute";
  newElement.style.top = `${newPositionY}px`;
  newElement.style.left = `${newPositionX}px`;

  newElement.addEventListener("dragstart", onDragStart);

  newElement.dataset.isSelected = false;
  newElement.addEventListener("click", (e) => {
    if (newElement.dataset.isSelected == "true") {
      newElement.blur();
      newElement.style.border = "none";
      newElement.dataset.isSelected = false;
    } else {
      newElement.focus();
      newElement.style.border = "1px dashed black";
      newElement.dataset.isSelected = true;
    }
  });

  document.addEventListener("keydown", (e) => {
    if (e.code === "Delete" && newElement.dataset.isSelected == "true") {
      console.log(`removed ${newElement.id}`);
      newElement.remove();
    }
  });

  return newElement;
}