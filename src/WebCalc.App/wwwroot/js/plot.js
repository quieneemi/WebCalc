const plot = document.getElementById('plot')
const yMin = document.getElementById('yMin')
const yMax = document.getElementById('yMax')
const xMin = document.getElementById('xMin')
const xMax = document.getElementById('xMax')

const config = {
    type: 'line',
    options: {
        plugins: {
            legend: {
                display: false
            }
        },
        scales: {
            x: {
                display: true
            },
            y: {
                display: true,
                suggestedMin: yMin.value,
                suggestedMax: yMax.value
            }
        }
    },
    data: {
        datasets: [{
            tension: 0.4
        }]
    }
}

const chart = new Chart(plot, config)

window.onload = function () {
    redraw();
}

function redraw () {
    let data = {
        expression: $("#expression").val(),
        xMin: xMin.value,
        xMax: xMax.value,
        yMin: yMin.value,
        yMax: yMax.value
    }
    
    $.ajax({
        type: "POST",
        url: "/Home/Plot",
        data: data,
        success: updateChartData,
        error: () => {
            alert('Something went wrong')
        }
    })
}

function updateChartData(data) {
    let labels = []
    for (let i = 0; i < data.length; i++) {
        labels[i] = Math.round(data[i].x * 4) / 4
    }
    chart.data.labels = labels
    chart.data.datasets[0].data = data
    chart.update()
}
