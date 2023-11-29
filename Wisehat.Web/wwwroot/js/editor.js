window.Widget = class Widget {
  constructor(id, description, widgetType, position, size, content, backgroundColor, borderColor) {
    this.id = id;
    this.description = description;
    this.widgetType = widgetType;
    this.position = position;
    this.size = size;
    this.content = content;
    this.backgroundColor = backgroundColor;
    this.borderColor = borderColor;
  }
}

var connection = new signalR.HubConnectionBuilder().withUrl("/webprojects").build();

document.getElementById("save-btn").disabled = true;

connection.start().then(() => {
  document.getElementById("save-btn").disabled = false;
}).catch((err) => {
  return console.error(err.toString());
});

document.getElementById("save-btn").addEventListener("click", (event) => {
  var widgets = Array.from(
    document.getElementById("canvas").children,
    (element) => createWidgetFromElement(element)
  );
  console.log(widgets);

  connection.invoke("ServerReceiveWidgets", widgets).catch((err) => {
    return console.error(err.toString());
  });
  event.preventDefault();
});

function createWidgetFromElement(element) {
  var location = element.getBoundingClientRect();

  return new Widget(
    element.id,
    null,
    element.id.split("-")[0],
    [location.left, location.top],
    [element.clientWidth, element.clientHeight],
    element.innerHTML,
    element.style.backgroundColor,
    element.style.borderColor
  );
}