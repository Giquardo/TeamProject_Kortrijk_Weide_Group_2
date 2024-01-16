import React, { useEffect, useState } from 'react';
import Chart from 'react-apexcharts';
import logo from './logo.svg';
import './App.css';

function App() {
  const [data, setData] = useState(null);

  useEffect(() => {
    fetch('http://localhost:5000/api/data')
      .then(response => response.json())
      .then(data => setData(data))
      .catch(error => console.error('Error:', error));
  }, []);

  const chartData = {
    options: {
      plotOptions: {
        bar: {
          horizontal: false,
          columnWidth: '55%',
          endingShape: 'rounded'
        },
      },
      dataLabels: {
        enabled: false
      },
      xaxis: {
        categories: data ? data.map(item => item.period) : [],
      },
      title: {
        text: 'Data Overview',
        align: 'left'
      },
      tooltip: {
        theme: 'dark'
      },
      theme: {
        mode: 'light',
        palette: 'palette1',
        monochrome: {
          enabled: false,
          color: '#255aee',
          shadeTo: 'light',
          shadeIntensity: 0.65
        },
      },
    },
    series: [
      {
        name: 'Consumption',
        data: data ? data.map(item => item.consumption) : [],
      },
      {
        name: 'Production',
        data: data ? data.map(item => item.production) : [],
      },
    ],
  };

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        {data ? (
          <div>
            <h1>Data from API:</h1>
            <Chart
              options={chartData.options}
              series={chartData.series}
              type="bar"
              width="500"
              className="custom-chart"
            />
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