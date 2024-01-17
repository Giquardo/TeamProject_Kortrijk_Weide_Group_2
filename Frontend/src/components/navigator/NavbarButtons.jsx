import React from "react";
import "./Navbar.css";

export const PrevButton = ({ enabled, onClick }) => (
  <div className="containerprev">
    <button className="previous" onClick={onClick} disabled={!enabled}>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="72.681"
        height="226.993"
        viewBox="0 0 72.681 226.993"
      >
        <path
          id="Icon_ion-arrow-left-b"
          data-name="Icon ion-arrow-left-b"
          d="M74.583,8.807l-.8,1.277L14.514,106.556c-2.007,3.263-3.257,8.158-3.257,13.62s1.288,10.357,3.257,13.62l59.16,96.4.985,1.632a4.318,4.318,0,0,0,3.3,1.915c3.3,0,5.984-5.249,5.984-11.775V18.525C83.938,12,81.249,6.75,77.954,6.75A4.378,4.378,0,0,0,74.583,8.807Z"
          transform="translate(-11.257 -6.75)"
          fill="#9a9a9a"
        />
      </svg>
    </button>
  </div>
);

export const NextButton = ({ enabled, onClick }) => (
  <div className="containernext">
    <button className="next" onClick={onClick} disabled={!enabled}>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="72.681"
        height="226.993"
        viewBox="0 0 72.681 226.993"
      >
        <path
          id="Icon_ion-arrow-left-b"
          data-name="Icon ion-arrow-left-b"
          d="M74.583,8.807l-.8,1.277L14.514,106.556c-2.007,3.263-3.257,8.158-3.257,13.62s1.288,10.357,3.257,13.62l59.16,96.4.985,1.632a4.318,4.318,0,0,0,3.3,1.915c3.3,0,5.984-5.249,5.984-11.775V18.525C83.938,12,81.249,6.75,77.954,6.75A4.378,4.378,0,0,0,74.583,8.807Z"
          transform="translate(83.938 233.743) rotate(180)"
          fill="#9a9a9a"
        />
      </svg>
    </button>
  </div>
);
