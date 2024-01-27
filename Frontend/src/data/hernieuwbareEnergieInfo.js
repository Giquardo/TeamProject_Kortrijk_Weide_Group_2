import warmtenet from '../Images/HernieuwbareEnergie/warmtenet_gif.gif';
import wkk from '../Images/HernieuwbareEnergie/wkk_gif.gif';
import zonneenergie from '../Images/HernieuwbareEnergie/zonneenergie_gif.gif';
const hernieuwbareEnergieInfo = [
  {
    id: 1,
    title: 'Zonne energie',
    naam: 'PV',
    description: 'De zon schijnt op de zonnepanelen en genereert zo stroom die wordt doorgegeven aan het gebouw.',
    // Add more properties as needed
    image: zonneenergie,
    imageAlt: 'Zonne energie',
  },
  {
    id: 2,
    title: 'Warmtekracht-koppeling',
    naam: 'WKK',
    description: 'Warmtekrachtkoppeling (WKK) wekt tegelijkertijd elektriciteit en warmte op uit één bron, zoals aardgas, voor meer efficiëntie en duurzaamheid.',
    // Add more properties as needed
    image: wkk,
    imageAlt: 'Warmtekracht-koppeling',
  },
  {
    id: 3,
    title: 'Warmtenet',
    naam: 'Warmtenet',
    description: 'Een warmtenet transporteert centraal opgewekte warmte naar gebouwen voor verwarming en warm water, wat efficiëntie bevordert en een duurzaam alternatief biedt voor individuele verwarmingssystemen.',
    // Add more properties as needed
    image: warmtenet,
    imageAlt: 'Warmtenet',
  },
];

export default hernieuwbareEnergieInfo;