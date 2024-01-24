import React, { useEffect, useState } from "react";
import "./EnergieStroomGebouw.css";
import Chart from "react-apexcharts";

const EnergieStroomGebouw = ({ info }) => {
  const [data, setData] = useState(null);

  useEffect(() => {
    fetch(`http://localhost:5000/api/Buildingdata/buildingspecific/${info.id}`)
      .then((response) => response.json())
      .then((data) => {
        if (
          data &&
          data.buildingspecificoverview &&
          Array.isArray(data.buildingspecificoverview)
        ) {
          setData(data.buildingspecificoverview);
        } else {
          console.error("Unexpected API response:", data);
        }
      })
      .catch((error) => console.error("Error fetching data:", error));
  }, [info.id]);
  // Use the info prop directly
  const building = info;

  // Define your chart options and series
  const options = {
    chart: {
      type: "bar",
      height: 300,
    },
    plotOptions: {
      bar: {
        horizontal: true,
      },
    },
    xaxis: {
      categories: ["Week", "Maand", "Jaar"],
      labels: {
        style: {
          colors: ["#ffffff"],
          fontSize: "10px",
        },
      },
    },
    yaxis: {
      labels: {
        style: {
          colors: ["#ffffff"],
          fontSize: "16px",
        },
      },
    },
    colors: ["#ffffff", "#e6007e"],
    dataLabels: {
      enabled: true,
      textAnchor: "start", // Position the labels next to the bars
      style: {
        colors: ["#000000"],
        fontSize: "16px",
      },
    },
  };

  // Use the fetched data to set the data for the chart series
  const series = data
    ? [
        {
          name: "Verbruik",
          data: data
            .filter((item) => item.type === "Realtime")
            .map((item) => item.consumption),
        },
        {
          name: "Referentie",
          data: data
            .filter((item) => item.type === "Referentie")
            .map((item) => item.consumption),
        },
      ]
    : [];

  const dailyRealtime =
    data &&
    data.find((item) => item.type === "Realtime" && item.period === "Week");

  return (
    <div className="energie-stroom-gebouw-container">
      <h1 className="title title_extra">Energie Stroom</h1>
      <div className="building-container">
        <img
          className="building-image"
          src={building.image}
          alt={building.name}
        />
        <h2 className="building-title">{building.name}</h2>
      </div>
      <div className="energiestroom-info-container">
        <div className="energiestroom-circle-container">
          <div className="energiestroom-circle">
            Realtime Verbruik
            <div className="circle-variable">
              {dailyRealtime
                ? `${dailyRealtime.consumption} kWh`
                : "Loading..."}
            </div>
          </div>
          <div className="energiestroom-circle">
            Productie
            <div className="circle-variable">
              {dailyRealtime ? `${dailyRealtime.production} kWh` : "Loading..."}
            </div>
          </div>
        </div>
        <div className="chart-container">
          <Chart
            className="chart"
            options={options}
            series={series}
            type="bar"
            height={350}
          />
        </div>
      </div>
    </div>
  );
};

export default EnergieStroomGebouw;
