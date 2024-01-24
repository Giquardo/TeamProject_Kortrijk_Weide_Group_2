// Initiële vragen voor de quiz
const vragen = [
  {
    id: 1,
    vraag: "Wat is de belangrijkste functie van warmtekrachtkoppeling (WKK) in Kortrijk Weide?",
    antwoorden: [
      { key: 'A', text: "Elektriciteit genereren" },
      { key: 'B', text: "Warmte leveren voor gebouwen" },
      { key: 'C', text: "Water zuiveren" },
      { key: 'D', text: "Voedselbereiding" }
    ],
    correctAntwoord: "A",
    verklaring: "WKK produceert elektriciteit en warmte voor gebouwen."
  }, 
  {
    id: 2,
    vraag: "Wat is een van de voordelen van hernieuwbare energiebronnen zoals zonne-energie?",
    antwoorden: [
      { key: 'A', text: "Ze zijn onuitputtelijk" },
      { key: 'B', text: "Ze veroorzaken geen uitstoot van broeikasgassen" },
      { key: 'C', text: "Ze zijn duurder dan fossiele brandstoffen" },
      { key: 'D', text: "Ze zijn altijd beschikbaar, ongeacht het weer" }
    ],
    correctAntwoord: "A",
    verklaring: "Ze steunen op duurzame, onuitputtelijke natuurlijke processen, in tegenstelling tot fossiele brandstoffen."
  },
  {
    id: 3,
    vraag: "Wat is het belangrijkste doel van het warmtenet op de Kortrijk Weide-site?",
    antwoorden: [
      { key: 'A', text: "Het opwekken van elektriciteit" },
      { key: 'B', text: "Het verwarmen van gebouwen" },
      { key: 'C', text: "Het koelen van gebouwen" },
      { key: 'D', text: "Het opslaan van energie" }
    ],
    correctAntwoord: "B",
    verklaring: "Het warmtenet op Kortrijk Weide zal warmte leveren aan gebouwen."
  },
  {
    id: 4,
    vraag: "Welke hernieuwbare energiebron wordt vaak gebruikt in gebouwen om warmte en elektriciteit op te wekken?",
    antwoorden: [
      { key: 'A', text: "Aardwarmte" },
      { key: 'B', text: "Windenergie" },
      { key: 'C', text: "Zonne-energie" },
      { key: 'D', text: "Kolen" }
    ],
    correctAntwoord: "C",
    verklaring: "Zonnepanelen worden vaak gebruikt om zonne-energie op te wekken."
  }
];

export default vragen;