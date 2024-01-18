import gebouw_A from '../Images/Buildings/gebouw_A.png';
import penta from '../Images/Buildings/penta.png';
import vegitec from '../Images/Buildings/vegitec.png';
import lago from '../Images/Buildings/lago.png';
import hangar from '../Images/Buildings/hangar.png';

const energieStroomGebouwInfo = [
  {
    id: 'KWE_A',
    name: 'KWE.A',
    image: gebouw_A,
  },
  {
    id: 'KWE_P',
    name: 'KWE.P',
    image: penta,
  },
  {
    id: 'VEG_i_TEC',
    name: 'VEG-i-TEC',
    image: vegitec,
  },
  {
    id: 'LAGO',
    name: 'LAGO',
    image: lago,
  },
  {
    id: ['Hangar_K', 'JC_Tranzit', 'MC_Track', 'Salie_Tricolor'],
    name: 'Hangar',
    image: hangar,
  },
];

export default energieStroomGebouwInfo;
