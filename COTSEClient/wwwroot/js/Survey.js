var chart_list = {};

var chart_data_format = (chart_type, label, labels = [], chart_datas = [], backgroundColor = [], border_color = "#fff") => {
    data = {};
    if (chart_type == "pie" || chart_type == "polarArea" || chart_type == "bar") {
        data = {
            labels: [...labels],
            datasets: [{
                label: label,
                data: [...chart_datas],
                backgroundColor: [...backgroundColor],
                borderColor: border_color,
                hoverOffset: 4
            }]
        };
        return data;
    }
    return null;
};

var sentiment_chart = (data, chart_type = "polarArea") => {
    let labels = Object.keys(data).slice(2);
    let values = Object.values(data).slice(2);
    var canva = $("#sentiment_chart")[0];
    var canva_label = "sentiment chart";
    var color_list = random_rpg_list(labels);
    var chart_data = chart_data_format(chart_type, canva_label, labels, values, color_list);
    var options = {
        responsive: true,
        scales: {
            r: {
                pointLabels: {
                    display: true,
                    centerPointLabels: true,
                    font: {
                        size: 18
                    }
                }
            }
        },
        plugins: {
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: 'Chart.js Polar Area Chart With Centered Point Labels'
            }
        }
    };
    create_chart(canva, chart_data, options, chart_type);
};

var create_chart = (canva, data = {}, options = {}, chart_type = "bar") => {
    var config = {
        type: chart_type,
        data: data,
        options: options
    };
    var chart = new Chart(canva, config);
    let chartId = canva.id;
    chart_list[chartId] = { chart: chart };
};

var delete_chart = (chartId) => {
    if (chart_list[chartId]) {
        var chart = chart_list[chartId].chart
        chart.destroy()
        delete chart_list[chartId]
    }
};

var random_rpg_list = (label) => {
    list_color = [];
    label.forEach(() => {
        var r = Math.floor(Math.random() * 256); // Random value for red (0-255)
        var g = Math.floor(Math.random() * 256); // Random value for green (0-255)
        var b = Math.floor(Math.random() * 256); // Random value for blue (0-255)
        list_color.push(`rgb(${r},${g},${b})`);
    });
    return list_color;
};

var changeSentimentChart = () => {

    var chart = $("#sentiment_chart")[0];
    delete_chart(chart.id);
    var chart_type = $("#selectChart").val();
    sentiment_chart(sentiment_data, chart_type);
};

$(() => {
    if (sentiment_data != null) {
        sentiment_chart(sentiment_data);
    }
});
