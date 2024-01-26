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
      fetch(
        `http://localhost:5000/api/Buildingdata/buildingspecific/${info.id}`
      ),
      fetch(`http://localhost:5000/api/LiveData/Liveoverview/${info.id}`),
    ])
      .then(async ([buildingRes, liveRes]) => {
        const buildingData = await buildingRes.json();
        let liveData = await liveRes.json();

        // Flatten the liveData object into an array and add a building property to each data object
        liveData = Object.values(liveData.liveoverview).flatMap(
          (buildingData) =>
            Object.entries(buildingData).flatMap(([building, data]) =>
              data.map((item) => ({ ...item, building }))
            )
        );

        // Combine or process the data as needed
        const combinedData = [
          ...buildingData.buildingspecificoverview,
          ...liveData,
        ];
        setData(combinedData);
        setIsLoadingConsumption(false);
        setIsLoadingProduction(false);

        // Print the data
        console.log(combinedData);
      })
      .catch((error) => {
        console.error("Error fetching data:", error);
        setIsLoadingConsumption(false);
        setIsLoadingProduction(false);
      });
  }, [info.id]);

  // Use the info prop directly
  const building = info;

  // Define your chart options and series
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
      categories: Array.from({ length: 30 }, (_, i) => (i + 1).toString()),
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
      enabled: true,
      textAnchor: "middle", // Position the labels next to the bars
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
          name: "Verbruik per dag",
          data: data
            .filter((item) => item.type === "Data")
            .map((item) => item.consumption),
        },
      ]
    : [];

  // Find the Realtime data for the current building
  const realtimeConsumption = data?.find(
    (item) => item.type === "Realtime" && item.msrExtra === "Consumption"
  );
  const realtimeProduction = data?.find(
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
