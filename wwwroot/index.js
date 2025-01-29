window.GetBoundingBoxByID = (elementID) => {
    let rect = document.getElementById(elementID).getBoundingClientRect();
    return { Left: rect.left, Top: rect.top };
}

window.keyPressHandler = {
    initialize: (dotNetReference) => {
        window.addEventListener("keydown", (event) => {
            dotNetReference.invokeMethodAsync("OnKeyDown", event.key);
        });
        window.addEventListener("keyup", (event) => {
            dotNetReference.invokeMethodAsync("OnKeyUp", event.key);
        });
    }
};