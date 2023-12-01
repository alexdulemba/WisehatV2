var connection = new signalR.HubConnectionBuilder().withUrl("/webprojects").build();

document.getElementById("save-btn").disabled = true;

connection.start().then(() => {
  document.getElementById("save-btn").disabled = false;
}).catch((err) => {
  return console.error(err.toString());
});

function createWidgetFromElement(element) {
  var location = element.getBoundingClientRect();

  return new window.Widget(
    element.id.split('_')[1],
    null,
    element.dataset.widgetType,
    null,
    { "x": element.clientWidth, "y": element.clientHeight },
    element.innerHTML,
    element.style.backgroundColor,
    element.style.borderColor,
    element.dataset.relativeLocation
  );
}