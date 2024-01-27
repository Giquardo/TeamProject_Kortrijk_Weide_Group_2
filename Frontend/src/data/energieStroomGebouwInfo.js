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
    productionId: 194,
    consumptionId: 279,

  },
  {
    id: 'KWE_P',
    name: 'KWE.P',
    image: penta,
    productionId: 415,
    consumptionId: 416,
  },
  {
    id: 'VEG_i_TEC',
    name: 'VEG-I-TEC',
    image: vegitec,
    productionId: 361,
    consumptionId: 362,
  },
  {
    id: 'LAGO',
    name: 'LAGO',
    image: lago,
    productionId: 0,
    consumptionId: 0,
  },
  {
    id: ['Hangar_K'],
    name: 'Hangar K',
    image: hangar,
    productionId: 401,
    consumptionId: 0,
  },
  {
    id: ['JC_Tranzit'],
    name: 'JC Tranzit',
    image: hangar,
    productionId: 0,
    consumptionId: 0,
  },
  {
    id: ['Salie_Tricolor'],
    name: 'Salie Tricolor',
    image: hangar,
    productionId: 0,
    consumptionId: 0,
  },
];

export default energieStroomGebouwInfo;
