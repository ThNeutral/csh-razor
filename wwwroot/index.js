window.GetBoundingBoxByID = (elementID) => {
    let rect = document.getElementById(elementID).getBoundingClientRect();
    return { Left: rect.left, Top: rect.top };
}