const expression = document.getElementById('expression');

function addSyllable(syllable) {
    expression.value += syllable;
}

function subtractSyllable() {
    let text = expression.value;
    let amount = 1;

    if (text.length === 0) return;

    if (text.endsWith('ln'))
        amount = 2;

    if (text.match('(sin|cos|tan|log|mod)$'))
        amount = 3;

    if (text.match('(sqrt|acos|asin|atan)$'))
        amount = 4;

    expression.value = text.slice(0, -amount);
}

function clearExpression() {
    expression.value = '';
}

function calculate() {
    let data = {
        expression: expression.value,
        xValue: $("#xValue").val()
    }

    $.ajax({
        type: "POST",
        url: "/Home/Calc",
        data: data,
        success: (response) => {
            expression.value = response;
        },
        error: () => {
            alert('Something went wrong');
        }
    });
}
