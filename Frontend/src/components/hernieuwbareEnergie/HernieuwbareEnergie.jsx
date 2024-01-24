import React, { useEffect, useState } from "react";
import "./HernieuwbareEnergie.css";
import "../General.css";

const Circle = ({ title, value }) => (
  <div className="circle">
    <p className="value-type">{title}</p>
    <p className="value">{value ? `${value} kWh` : "Loading..."}</p>
  </div>
);

const HernieuwbareEnergie = ({ info }) => {
  const [data, setData] = useState({
    injectie: 0,
    eigenverbruik: 0,
    productie: 0,
  });

  useEffect(() => {
    fetch(`http://localhost:5000/api/zuinigedata/zuinigeoverview/${info.naam}`)
      .then((response) => response.json())
      .then((data) => setData(data.zuinigeoverview[0]))
      .catch((error) => console.error("Error:", error));
  }, [info.naam]);

  return (
    <>
      <div className="hernieuwbareEnergie-container">
        <h1 className="title title_extra">{info.title}</h1>
        <p className="hernieuwbareEnergie-description">{info.description}</p>
        {/* Add this line */}
        <img className="energie-image" src={info.image} alt={info.imageAlt} />
        <div className="circles">
          <div className="circle-container">
            <Circle title="Consumptie" value={data.consumption} />
            <Circle title="Eigen Verbruik" value={data.eigenVerbruik} />
          </div>
          <Circle title="Productie" value={data.production} />
        </div>
      </div>
    </>
  );
};

export default HernieuwbareEnergie;
