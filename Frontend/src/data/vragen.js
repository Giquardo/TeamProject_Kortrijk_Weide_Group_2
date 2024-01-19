// Initiële vragen voor de quiz
const vragen = [
  {
    id: 1,
    vraag: "Wat is de belangrijkste functie van warmtekrachtkoppeling (WKK) in Kortrijk Weide?",
    antwoorden: [
      { key: 'a', text: "Elektriciteit genereren" },
      { key: 'b', text: "Warmte leveren voor gebouwen" },
      { key: 'c', text: "Water zuiveren" },
      { key: 'd', text: "Voedselbereiding" }
    ],
    correctAntwoord: "a",
    verklaring: "WKK produceert elektriciteit en warmte voor gebouwen."
  }, 
  {
    id: 2,
    vraag: "Wat is een van de voordelen van hernieuwbare energiebronnen zoals zonne-energie?",
    antwoorden: [
      { key: 'a', text: "Ze zijn onuitputtelijk" },
      { key: 'b', text: "Ze veroorzaken geen uitstoot van broeikasgassen" },
      { key: 'c', text: "Ze zijn goedkoper dan fossiele brandstoffen" },
      { key: 'd', text: "Ze zijn altijd beschikbaar, ongeacht het weer" }
    ],
    correctAntwoord: "a",
    verklaring: "ze zijn afhankelijk van natuurlijke processen die voortdurend plaatsvinden en niet worden uitgeput bij gebruik, zoals bijvoorbeeld fossiele brandstoffen "
  },
  {
    id: 3,
    vraag: "Wat is het belangrijkste doel van het warmtenet op de Kortrijk Weide-site?",
    antwoorden: [
      { key: 'a', text: "Het opwekken van elektriciteit" },
      { key: 'b', text: "Het verwarmen van gebouwen" },
      { key: 'c', text: "Het koelen van gebouwen" },
      { key: 'd', text: "Het opslaan van energie" }
    ],
    correctAntwoord: "b",
    verklaring: "Het warmtenet op Kortrijk Weide zal warmte leveren aan gebouwen."
  },
  {
    id: 4,
    vraag: "Welke hernieuwbare energiebron wordt vaak gebruikt in gebouwen om warmte en elektriciteit op te wekken?",
    antwoorden: [
      { key: 'a', text: "Zonne-energie" },
      { key: 'b', text: "Windenergie" },
      { key: 'c', text: "Aardwarmte" },
      { key: 'd', text: "Kolen" }
    ],
    correctAntwoord: "a",
    verklaring: "Zonnepanelen worden vaak gebruikt om zonne-energie op te wekken."
  }
];

export default vragen;