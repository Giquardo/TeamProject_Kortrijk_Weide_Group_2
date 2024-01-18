import React from 'react';
import './EnergieStroomGebouw.css';
import Chart from 'react-apexcharts';

const EnergieStroomGebouw = ({ info, realtimeVerbruik, productie }) => {
  // Use the info prop directly
  const building = info;

    // Define your chart options and series
    const options = {
        chart: {
        type: 'bar',
        height: 300,
        },
        plotOptions: {
        bar: {
            horizontal: true,
        },
        },
        xaxis: {
        categories: ['Dag', 'Maand', 'Jaar'],
        },
        colors: ['#ffffff', '#e6007e'],
        dataLabels: {
            enabled: true,
            textAnchor: 'start', // Position the labels next to the bars
            style: {
              colors: ['#000000'], // Make the labels black
            },
          },
    };

    const series = [
        {
        name: 'Verbruik',
        data: [400, 430, 448],
        },
        {
        name: 'Referentie',
        data: [300, 350, 400],
        },
    ];

    return (
        <div className="energie-stroom-gebouw-container">
          <h1 className='title'>Energie Stroom</h1>
          <div className="building-container">
            <img className='building-image' src={building.image} alt={building.name} />
            <h2 className='building-title'>{building.name}</h2>
          </div>
          <div className='energiestroom-info-container'>
            <div className="energiestroom-circle-container">
              <div className="energiestroom-circle">
                Realtime Verbruik
                <div className="circle-variable">realtimeVerbruik</div>
              </div>
              <div className="energiestroom-circle">
                Productie
                <div className="circle-variable">productie</div>
              </div>
            </div>
            <div className="chart-container">
              <Chart options={options} series={series} type="bar" height={350} />
            </div>
          </div>
        </div>
      );
};

export default EnergieStroomGebouw;