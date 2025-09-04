var currentStats = null;

async function loadUserStats(userName) {
  console.log(userName);
    if (!userName) {
        d3.select("#chart").html("<p>Please select a user.</p>");
        return;
    }

    const response = await fetch(`/Dashboards/GetUserStats/${userName}`);
    if (response.status == "404") {
      d3.select("#chart").html("<p>Not Found User's Data.</p>");
      return;
    }
    const stats = await response.json();
    currentStats = stats;
    reloadChart();
}

function reloadChart() {
    const chartType = document.getElementById("chartType").value;
    if (!currentStats){
      return;
    }

    if (chartType === "bar") renderBarChart(currentStats);
    else if (chartType === "line") renderLineChart(currentStats);
    else if (chartType === "pie") renderPieChart(currentStats);
}

function getDataset(stats) {
    return [
        { name: "Voice Calls", value: Number(stats.voiceCallCount) || 0 },
        { name: "Video Calls", value: Number(stats.videoCallCount) || 0 },
        { name: "Mock Interviews", value: Number(stats.mockInterviewCount) || 0 },
        { name: "Applied Jobs", value: Number(stats.appliedJob) || 0 }
    ];
}
function renderBarChart(stats) {
    const dataset = getDataset(stats);
    d3.select("#chart").html(""); // clear old chart

    const width = 500, height = 300, margin = 40;
    const svg = d3.select("#chart")
        .append("svg")
        .attr("width", width)
        .attr("height", height);

    const x = d3.scaleBand()
        .domain(dataset.map(d => d.name))
        .range([margin, width - margin])
        .padding(0.2);

    const y = d3.scaleLinear()
        .domain([0, d3.max(dataset, d => d.value)])
        .nice()
        .range([height - margin, margin]);

    svg.append("g")
        .attr("transform", `translate(0,${height - margin})`)
        .call(d3.axisBottom(x));

    svg.append("g")
        .attr("transform", `translate(${margin},0)`)
        .call(d3.axisLeft(y));

    svg.selectAll(".bar")
        .data(dataset)
        .enter()
        .append("rect")
        .attr("x", d => x(d.name))
        .attr("y", d => y(d.value))
        .attr("width", x.bandwidth())
        .attr("height", d => height - margin - y(d.value))
        .attr("fill", "steelblue");
}

function renderLineChart(stats) {
    const dataset = getDataset(stats);
    d3.select("#chart").html("");

    const width = 500, height = 300, margin = 40;
    const svg = d3.select("#chart")
        .append("svg")
        .attr("width", width)
        .attr("height", height);

    const x = d3.scalePoint()
        .domain(dataset.map(d => d.name))
        .range([margin, width - margin]);

    const y = d3.scaleLinear()
        .domain([0, d3.max(dataset, d => d.value)])
        .nice()
        .range([height - margin, margin]);

    const line = d3.line()
        .x(d => x(d.name))
        .y(d => y(d.value));

    svg.append("g")
        .attr("transform", `translate(0,${height - margin})`)
        .call(d3.axisBottom(x));

    svg.append("g")
        .attr("transform", `translate(${margin},0)`)
        .call(d3.axisLeft(y));

    svg.append("path")
        .datum(dataset)
        .attr("fill", "none")
        .attr("stroke", "steelblue")
        .attr("stroke-width", 2)
        .attr("d", line);

    svg.selectAll("circle")
        .data(dataset)
        .enter()
        .append("circle")
        .attr("cx", d => x(d.name))
        .attr("cy", d => y(d.value))
        .attr("r", 4)
        .attr("fill", "steelblue");
}

function renderPieChart(stats) {
    const dataset = getDataset(stats);
    d3.select("#chart").html("");

    const width = 400, height = 400, radius = Math.min(width, height) / 2;

    const svg = d3.select("#chart")
        .append("svg")
        .attr("width", width)
        .attr("height", height)
        .append("g")
        .attr("transform", `translate(${width / 2},${height / 2})`);

    const color = d3.scaleOrdinal(d3.schemeCategory10);
    const pie = d3.pie().value(d => d.value);
    const arc = d3.arc().innerRadius(0).outerRadius(radius);

    svg.selectAll("path")
        .data(pie(dataset))
        .enter()
        .append("path")
        .attr("d", arc)
        .attr("fill", d => color(d.data.name));

    svg.selectAll("text")
        .data(pie(dataset))
        .enter()
        .append("text")
        .attr("transform", d => `translate(${arc.centroid(d)})`)
        .attr("dy", "0.35em")
        .style("text-anchor", "middle")
        .text(d => d.data.name);
}

window.loadUserStats = loadUserStats;
window.reloadChart = reloadChart;
window.renderLineChart = renderLineChart;
window.renderBarChart = renderBarChart;
window.renderPieChart = renderPieChart;
