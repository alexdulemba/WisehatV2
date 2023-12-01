export class Widget {
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