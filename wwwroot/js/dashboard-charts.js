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
        dark: '#7a2e42',
        mid: '#a84358',
        base: '#d16b7f',
        light: '#e895a3',
        pale: '#fdecef',
        gold: '#c9a227'
    };

    const fontFamily = "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif";

    Chart.defaults.font.family = fontFamily;
    Chart.defaults.color = '#5a6d62';
    Chart.defaults.plugins.legend.labels.usePointStyle = true;
    Chart.defaults.plugins.legend.labels.padding = 14;

    const pieOptions = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: { position: 'bottom' },
            tooltip: {
                backgroundColor: green.dark,
                padding: 12,
                cornerRadius: 8
            }
        }
    };

    const doughnutOptions = {
        ...pieOptions,
        cutout: '62%'
    };

    /** Chart 1 — full pie; show placeholder when all counts are zero */
    function pieDataset(labels, values, colors, emptyLabel) {
        const total = values.reduce((sum, n) => sum + n, 0);
        if (total === 0) {
            return {
                labels: [emptyLabel],
                data: [1],
                backgroundColor: ['#e8cdd4'],
                borderColor: '#fff',
                placeholder: true
            };
        }
        return {
            labels,
            data: values,
            backgroundColor: colors,
            borderColor: '#fff',
            placeholder: false
        };
    }

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
                grid: { color: 'rgba(122, 46, 66, 0.08)' }
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
                grid: { color: 'rgba(122, 46, 66, 0.08)' }
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

    const statusPie = pieDataset(
        data.statusLabels,
        data.statusValues,
        [green.mid, green.light],
        'No chemicals yet'
    );

    makeChart('chartStatus', {
        type: 'pie',
        data: {
            labels: statusPie.labels,
            datasets: [{
                data: statusPie.data,
                backgroundColor: statusPie.backgroundColor,
                borderColor: statusPie.borderColor,
                borderWidth: 3,
                hoverOffset: statusPie.placeholder ? 0 : 8
            }]
        },
        options: {
            ...pieOptions,
            plugins: {
                ...pieOptions.plugins,
                legend: {
                    position: 'bottom',
                    labels: {
                        usePointStyle: true,
                        padding: 14,
                        generateLabels(chart) {
                            const ds = chart.data.datasets[0];
                            const total = ds.data.reduce((a, b) => a + b, 0);
                            return chart.data.labels.map((label, i) => ({
                                text: statusPie.placeholder
                                    ? label
                                    : `${label}: ${ds.data[i]} (${total ? Math.round((ds.data[i] / total) * 100) : 0}%)`,
                                fillStyle: Array.isArray(ds.backgroundColor)
                                    ? ds.backgroundColor[i]
                                    : ds.backgroundColor,
                                strokeStyle: '#fff',
                                lineWidth: 1,
                                hidden: false,
                                index: i
                            }));
                        }
                    }
                },
                tooltip: {
                    ...pieOptions.plugins.tooltip,
                    callbacks: {
                        label(context) {
                            if (statusPie.placeholder) {
                                return 'Add an SDS to see status breakdown';
                            }
                            const sum = context.dataset.data.reduce((a, b) => a + b, 0);
                            const value = context.parsed;
                            const pct = sum ? Math.round((value / sum) * 100) : 0;
                            return `${context.label}: ${value} (${pct}%)`;
                        }
                    }
                }
            }
        }
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
                    'rgba(168, 67, 88, 0.85)',
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
                backgroundColor: 'rgba(201, 77, 102, 0.15)',
                fill: true,
                pointBorderColor: green.base,
                pointBackgroundColor: '#fff'
            }]
        },
        options: lineOptions
    });
})();
