import React, { useEffect, useState } from 'react';
import logo from './logo.svg';
import './App.css';

function App() {
  const [data, setData] = useState(null);

  useEffect(() => {
    fetch('http://localhost:5000/api/data')
      .then(response => response.json())
      .then(data => {
        console.log(data);
        setData(data);
      })
      .catch(error => console.error('Error:', error));
  }, []);

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        {data ? (
          <div>
            <h1>Data from API:</h1>
            {data.map((item) => (
              <div key={item.period}>
                <h2>{item.period}</h2>
                <p>Consumption: {item.consumption}</p>
                <p>Production: {item.production}</p>
              </div>
            ))}
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