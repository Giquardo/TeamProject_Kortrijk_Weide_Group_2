import React from "react";
import { PrevButton, NextButton } from "./NavbarButtons";

const Navbar = () => {
  const handleNext = () => {
    console.log("next");
  };

  const handlePrev = () => {
    console.log("prev");
  };

  return (
    <div>
      <PrevButton onClick={handlePrev} />
      <NextButton onClick={handleNext} />
    </div>
  );
};

export default Navbar;
