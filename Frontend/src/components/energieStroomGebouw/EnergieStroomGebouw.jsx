import React, { useEffect, useState } from "react";
import "./EnergieStroomGebouw.css";
import Chart from "react-apexcharts";

const EnergieStroomGebouw = ({ info }) => {
  const [data, setData] = useState(null);
  const [isLoadingConsumption, setIsLoadingConsumption] = useState(true);
  const [isLoadingProduction, setIsLoadingProduction] = useState(true);

  useEffect(() => {
    setIsLoadingConsumption(true);
    setIsLoadingProduction(true);
    Promise.all([
      fetch(`http://localhost:5000/api/Buildingdata/buildingspecific/${info.id}`),
      fetch(`http://localhost:5000/api/LiveData/Liveoverview/${info.id}`),
    ])
      .then(async ([buildingRes, liveRes]) => {
        const buildingData = await buildingRes.json();
        let liveData = await liveRes.json();
  
        liveData = Object.values(liveData.liveoverview).flatMap(
          (buildingData) =>
            Object.entries(buildingData).flatMap(([building, data]) =>
              data.map((item) => ({ ...item, building }))
            )
        );
  
        const combinedData = {
        buildingspecificoverview: buildingData.buildingspecificoverview,
        dailyLastMonth: buildingData.dailyLastMonth,
        liveData: liveData,
        };
        setData(combinedData);
        setIsLoadingConsumption(false);
        setIsLoadingProduction(false);
      })
      .catch((error) => {
        console.error("Error fetching data:", error);
        setIsLoadingConsumption(false);
        setIsLoadingProduction(false);
      });
  }, [info.id]);

  const building = info;

  const options = {
    chart: {
      type: "bar",
      height: 300,
      width: 450,
    },
    plotOptions: {
      bar: {
        horizontal: false,
      },
    },
    xaxis: {
      type: 'datetime',
      labels: {
        style: {
          colors: ["#000000"],
          fontSize: "10px",
        },
        format: 'dd MMM',
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
      textAnchor: "middle",
      style: {
        colors: ["#000000"],
        fontSize: "16px",
      },
    },
    tooltip: {
      theme: 'dark',
      style: {
        colors: ['#000000']
      }
    }
  };

  const series = data
  ? [
      {
        name: "Verbruik laatste maand",
        data: data.dailyLastMonth.map((item) => [new Date(item.time), parseFloat(item.value)]),
      },
    ]
  : [];

  const realtimeConsumption = data?.liveData.find(
    (item) => item.type === "Realtime" && item.msrExtra === "Consumption"
  );
  const realtimeProduction = data?.liveData.find(
    (item) => item.type === "Realtime" && item.msrExtra === "Production"
  );
  

  return (
    <div className="energie-stroom-gebouw-container">
      <h1 className="title">Energie Stroom</h1>
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
              {isLoadingConsumption
                ? "Loading..."
                : realtimeConsumption
                ? `${realtimeConsumption.value} kWh`
                : "Geen data"}
            </div>
          </div>
          <div className="energiestroom-circle">
            Productie
            <div className="circle-variable">
              {isLoadingProduction
                ? "Loading..."
                : realtimeProduction
                ? `${realtimeProduction.value} kWh`
                : "Geen data"}
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
            width={700}
          />
        </div>
      </div>
    </div>
  );
};

export default EnergieStroomGebouw;