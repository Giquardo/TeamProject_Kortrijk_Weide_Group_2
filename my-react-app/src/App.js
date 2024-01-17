// App.js

import React, { useEffect, useState } from 'react';
import Chart from 'react-apexcharts';
import logo from './logo.svg';
import './App.css';

function App() {
  const [data, setData] = useState(null);
  const [buildingSpecificDataKWE_A, setBuildingSpecificDataKWE_A] = useState(null);
  const [buildingSpecificDataLAGO, setBuildingSpecificDataLAGO] = useState(null);

  useEffect(() => {
    // Fetch general data
    fetch('http://localhost:5000/api/data/overview')
      .then(response => response.json())
      .then(data => {
        console.log('Received data:', data);
        setData(data);
      })
      .catch(error => console.error('Error:', error));
  }, []);

  useEffect(() => {
    // Fetch building-specific data for KWE_A
    fetch('http://localhost:5000/api/data/buildingspecific/KWE_A')
      .then(response => response.json())
      .then(data => {
        console.log('Received building-specific data for KWE_A:', data);
        setBuildingSpecificDataKWE_A(data);
      })
      .catch(error => console.error('Error fetching building-specific data for KWE_A:', error));
  }, []);

  useEffect(() => {
    // Fetch building-specific data for LAGO
    fetch('http://localhost:5000/api/data/buildingspecific/LAGO')
      .then(response => response.json())
      .then(data => {
        console.log('Received building-specific data for LAGO:', data);
        setBuildingSpecificDataLAGO(data);
      })
      .catch(error => console.error('Error fetching building-specific data for LAGO:', error));
  }, []);

  const renderChart = (chartData, title) => (
    <div>
      <h1>{title}</h1>
      <Chart
        options={chartData.options}
        series={chartData.series}
        type="bar"
        width="500"
        className="custom-chart"
      />
    </div>
  );

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        {data ? (
          <div>
            {/* General Overview */}
            {renderChart(
              {
                options: {
                  // General Overview options...
                },
                series: [
                  {
                    name: 'Consumption',
                    data: data.generaloverview
                      ? data.generaloverview.map(item => item.consumption)
                      : [],
                  },
                  {
                    name: 'Production',
                    data: data.generaloverview
                      ? data.generaloverview.map(item => item.production)
                      : [],
                  },
                ],
              },
              'General Overview'
            )}

            {/* Production Overview */}
            {renderChart(
              {
                options: {
                  // Production Overview options...
                },
                series: [
                  {
                    name: 'Production',
                    data: data.productionoverview
                      ? data.productionoverview.map(item => item.production)
                      : [],
                  },
                  {
                    name: 'Injection',
                    data: data.productionoverview
                      ? data.productionoverview.map(item => item.injection)
                      : [],
                  },
                ],
              },
              'Production Overview'
            )}

            {/* Building-Specific Overview for KWE_A */}
            {buildingSpecificDataKWE_A && renderChart(
              {
                options: {
                  // Building-Specific options...
                },
                series: [
                  {
                    name: 'Consumption',
                    data: buildingSpecificDataKWE_A.buildingspecificoverview
                      ? buildingSpecificDataKWE_A.buildingspecificoverview.map(item => item.consumption)
                      : [],
                  },
                  {
                    name: 'Production',
                    data: buildingSpecificDataKWE_A.buildingspecificoverview
                      ? buildingSpecificDataKWE_A.buildingspecificoverview.map(item => item.production)
                      : [],
                  },
                ],
              },
              'KWE_A Overview'
            )}

            {/* Building-Specific Overview for LAGO */}
            {buildingSpecificDataLAGO && renderChart(
              {
                options: {
                  // Building-Specific options...
                },
                series: [
                  {
                    name: 'Consumption',
                    data: buildingSpecificDataLAGO.buildingspecificoverview
                      ? buildingSpecificDataLAGO.buildingspecificoverview.map(item => item.consumption)
                      : [],
                  },
                  {
                    name: 'Production',
                    data: buildingSpecificDataLAGO.buildingspecificoverview
                      ? buildingSpecificDataLAGO.buildingspecificoverview.map(item => item.production)
                      : [],
                  },
                ],
              },
              'LAGO Overview'
            )}
          </div>
        ) : (
          <p>Loading...</p>
        )}
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
    </div>
  );
}

export default App;
