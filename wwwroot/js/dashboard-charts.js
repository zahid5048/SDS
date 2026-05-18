(function () {
    const dataEl = document.getElementById('dashChartData');
    if (!dataEl || typeof Chart === 'undefined') return;

    let data;
    try {
        data = JSON.parse(dataEl.textContent);
    } catch (e) {
        return;
    }

    const green = {
        dark: '#0f3d24',
        mid: '#1a5c34',
        base: '#228b4a',
        light: '#3cb371',
        pale: '#e8f5e9',
        gold: '#c9a227'
    };

    const fontFamily = "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif";

    Chart.defaults.font.family = fontFamily;
    Chart.defaults.color = '#5a6d62';
    Chart.defaults.plugins.legend.labels.usePointStyle = true;
    Chart.defaults.plugins.legend.labels.padding = 14;

    const doughnutOptions = {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '62%',
        plugins: {
            legend: { position: 'bottom' },
            tooltip: {
                backgroundColor: green.dark,
                padding: 12,
                cornerRadius: 8
            }
        }
    };

    const barOptions = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: { display: false },
            tooltip: {
                backgroundColor: green.dark,
                padding: 12,
                cornerRadius: 8
            }
        },
        scales: {
            x: {
                grid: { display: false },
                ticks: { font: { size: 11 } }
            },
            y: {
                beginAtZero: true,
                ticks: { stepSize: 1, precision: 0 },
                grid: { color: 'rgba(34, 139, 74, 0.08)' }
            }
        }
    };

    const lineOptions = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: { display: false },
            tooltip: {
                backgroundColor: green.dark,
                padding: 12,
                cornerRadius: 8
            }
        },
        scales: {
            x: { grid: { display: false } },
            y: {
                beginAtZero: true,
                ticks: { stepSize: 1, precision: 0 },
                grid: { color: 'rgba(34, 139, 74, 0.08)' }
            }
        },
        elements: {
            line: { tension: 0.35, borderWidth: 3 },
            point: { radius: 4, hoverRadius: 6, borderWidth: 2, backgroundColor: '#fff' }
        }
    };

    function makeChart(canvasId, config) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;
        new Chart(canvas.getContext('2d'), config);
    }

    makeChart('chartStatus', {
        type: 'doughnut',
        data: {
            labels: data.statusLabels,
            datasets: [{
                data: data.statusValues,
                backgroundColor: [green.base, green.gold],
                borderColor: '#fff',
                borderWidth: 3,
                hoverOffset: 8
            }]
        },
        options: doughnutOptions
    });

    makeChart('chartSignal', {
        type: 'doughnut',
        data: {
            labels: data.signalLabels,
            datasets: [{
                data: data.signalValues,
                backgroundColor: ['#dc3545', '#f59e0b', '#94a3b8'],
                borderColor: '#fff',
                borderWidth: 3,
                hoverOffset: 8
            }]
        },
        options: doughnutOptions
    });

    makeChart('chartProgress', {
        type: 'bar',
        data: {
            labels: data.progressLabels,
            datasets: [{
                data: data.progressValues,
                backgroundColor: [
                    'rgba(148, 163, 184, 0.85)',
                    'rgba(201, 162, 39, 0.85)',
                    'rgba(46, 168, 95, 0.85)',
                    green.base
                ],
                borderRadius: 8,
                borderSkipped: false
            }]
        },
        options: barOptions
    });

    makeChart('chartMonthly', {
        type: 'line',
        data: {
            labels: data.monthlyLabels,
            datasets: [{
                data: data.monthlyValues,
                borderColor: green.base,
                backgroundColor: 'rgba(34, 139, 74, 0.12)',
                fill: true,
                pointBorderColor: green.base,
                pointBackgroundColor: '#fff'
            }]
        },
        options: lineOptions
    });
})();
