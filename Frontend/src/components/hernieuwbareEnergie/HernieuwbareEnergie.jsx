import React, { useEffect, useState } from "react";
import "./HernieuwbareEnergie.css";
import "../General.css";
import Chart from "react-apexcharts";

const HernieuwbareEnergie = ({ info }) => {
  const [data, setData] = useState(null);

  useEffect(() => {
    fetch(`http://localhost:5000/api/zuinigedata/zuinigeoverview/${info.naam}`)
      .then((response) => response.json())
      .then((data) => setData(data.zuinigeoverview))
      .catch((error) => console.error("Error:", error));
  }, [info.naam]);

  const options = {
    chart: {
      type: "bar",
      height: 300,
      width: 500,
    },
    plotOptions: {
      bar: {
        horizontal: false,
      },
    },
    xaxis: {
      categories: data ? data.map((item) => item.month) : [],
      labels: {
        style: {
          colors: ["#000000"],
          fontSize: "10px",
        },
      },
    },
    yaxis: {
      labels: {
        style: {
          colors: ["#000000"],
          fontSize: "16px",
        },
      },
    },
    legend: {
      show: true,
      position: "bottom",
      horizontalAlign: "center",
      showForSingleSeries: true,
    },
    colors: ["#e6007e"],
    dataLabels: {
      enabled: false,
      textAnchor: "start", // Position the labels next to the bars
      style: {
        colors: ["#000000"],
        fontSize: "16px",
      },
    },
  };

  const series = data
    ? [
        {
          name: "Maandelijkse productie",
          data: data.map((item) => parseFloat(item.production)),
        },
      ]
    : [];

  return (
    <>
      <div className="hernieuwbareEnergie-container">
        <h1 className="title title_extra">{info.title}</h1>
        <p className="hernieuwbareEnergie-description">{info.description}</p>
        <img className="energie-image" src={info.image} alt={info.imageAlt} />
        <div className="chart-container">
          <Chart
            className="chart"
            options={options}
            series={series}
            type="bar"
            height={350}
            width={650}
          />
        </div>
      </div>
    </>
  );
};

export default HernieuwbareEnergie;