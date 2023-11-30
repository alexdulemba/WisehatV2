export class Widget {
  constructor(id, description, widgetType, grabPosition, size, content, backgroundColor, borderColor, location) {
    this.id = id;
    this.description = description;
    this.widgetType = widgetType;
    this.grabPosition = grabPosition;
    this.size = size;
    this.content = content;
    this.backgroundColor = backgroundColor;
    this.borderColor = borderColor;
    this.location = location;
  }
}

window.Widget = Widget;