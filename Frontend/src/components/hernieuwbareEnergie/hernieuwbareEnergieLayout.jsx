import React, { useEffect, useState } from "react";
import "./HernieuwbareEnergie.css";
import zonneEnergie from "../../Images/zonneEnergie.png";

const Circle = ({ title, value }) => (
  <div className="circle">
    <p className="value-type">{title}</p>
    <p className="value">{value ? `${value} kW` : 'Loading...'}</p>
  </div>
);

const HernieuwbareEnergieLayout = ({ info }) => {
  const [data, setData] = useState({ injectie: 0, eigenverbruik: 0, productie: 0 });

  useEffect(() => {
    fetch(`http://localhost:5000/api/data/hernieuwbareEnergie/${info.naam}`)
      .then(response => response.json())
      .then(data => setData(data.totals))
      .catch(error => console.error('Error:', error));
  }, [info.naam]);

  return (
    <>
      <div className="hernieuwbareEnergie-container">
        <h1 className="hernieuwbareEnergie-title">{info.title}</h1>
        <p className="hernieuwbareEnergie-description">{info.description}</p> {/* Add this line */}
        <img className="energie-image" src={zonneEnergie} alt="zonneEnergie" />
        <div className="circles">
          <div className="circle-container">
            <Circle title="Injectie" value={data.Injectie} />
            <Circle title="Eigenverbruik" value={data.Productie_EigenVerbruik} />
          </div>
          <Circle title="Productie" value={data.Productie} />
        </div>
      </div>
    </>
  );
};

export default HernieuwbareEnergieLayout;