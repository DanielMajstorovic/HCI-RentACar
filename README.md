# RentACar

## Uvod

RentACar je desktop aplikacija za upravljanje poslovima iznajmljivanja vozila, dizajnirana da pojednostavi svakodnevne poslovne operacije rent-a-car agencija. Aplikacija pruža intuitivno korisničko iskustvo, omogućavajući efikasno upravljanje voznim parkom, rezervacijama, klijentima i finansijskim aspektima poslovanja.
Aplikacija je razvijena kao studentski projekat u okviru predmeta *Human-Computer Interaction* sa fokusom na intuitivan interfejs, pristupačnost i jednostavnu navigaciju.

Aplikacija podržava dva tipa korisničkih naloga:
- Administratorski
- Agentski
  
Pri čemu svaki od tipova naloga ima pristup specifičnim funkcionalnostima prilagođenim njihovim radnim potrebama. Administratorski nalozi su unaprijed kreirani u sistemu, dok agentski nalozi su upravljani od strane administratora.

RentACar je razvijen kao sveobuhvatno rješenje za upravljanje rent-a-car poslovanjem, omogućavajući jednostavno praćenje dostupnosti vozila, upravljanje iznajmljivanjima u različitim statusima (aktivno, rezervisano, otkazano, završeno), vođenje evidencije o klijentima, kao i izbor dodatnih opcija prilikom svakog iznajmljivanja.

---

## Tipovi korisničkih naloga

### Administratorski nalog
Administratori imaju najviši nivo pristupa sistemu i zaduženi su za:
- Upravljanje voznim parkom (dodavanje, uređivanje i uklanjanje vozila)
- Kreiranje i upravljanje agentskim nalozima
- Pregled svih iznajmljivanja u sistemu
- Kreiranje i upravljanje dodatnim opcijama iznajmljivanja
- Obrađivanje prijavljenih problema

### Agentski nalog
Agenti predstavljaju zaposlene osobe koje svakodnevno koriste aplikaciju za:
- Rad sa klijentima i unos njihovih podataka
- Upravljanje iznajmljivanjima vozila
- Pregled dostupnih vozila u sistemu
- Pregledanje dostupnosti vozila
- Prijavu problema

---

## Prijava na sistem

Početni ekran aplikacije prikazuje formu za prijavu koja je zajednička za obe vrste korisnika. Pristup funkcionalnostima sistema određen je tipom korisničkog naloga koji se koristi za prijavu.

![login-dark](https://github.com/user-attachments/assets/c1d4e987-fdce-41a9-9361-b541bf573fc4)

Korisnici unose svoje korisničko ime i lozinku, a nakon uspješne prijave, otvara se odgovarajući interfejs baziran na tipu njihovog korisničkog naloga.

---

## Agentski interfejs

Agenti nakon prijave dobijaju pristup agentskom dijelu sistema prilagođenom za rad sa klijentima i iznajmljivanjima.

### Upravljanje iznajmljivanjima

![agent-2-1](https://github.com/user-attachments/assets/94d50195-fb38-421c-bafa-67b0373200ed)

Agentima je omogućeno da:
- Pretražuju i pregledaju detalje postojećih iznajmljivanja

![agent-2-2](https://github.com/user-attachments/assets/d1642420-1cc0-4dcc-af0f-3fab3a971fec)
  
![agent-2-3](https://github.com/user-attachments/assets/9d7c56c0-e9b6-46e4-b3ac-21c8f373aa33)

**Brisanje iznajmljivanja**

![agent-2-5](https://github.com/user-attachments/assets/062b45b7-3597-456a-9d79-6db0ff4db836)

*Nakon brisanja*

![agent-2-6](https://github.com/user-attachments/assets/330aa980-c163-4503-8b5f-423e09c1d3fa)


- Mijenjaju status iznajmljivanja prema potrebi (npr. prilikom preuzimanja vozila agent prebacuje status za dato iznajmljivanje iz rezervisanog u aktivno stanje)

![agent-2-4](https://github.com/user-attachments/assets/b8055a66-ff91-49a0-9c5e-58b5f378ded9)

- Kreiraju nova iznajmljivanja za klijente iterativnim procesom:
    - Prvo se bira željeni vremenski okvir:
 
      ![agent-2-7](https://github.com/user-attachments/assets/df2327bd-f603-46d6-a110-7d2ac022cb9a)

    - Zatim se bira željeno dostupno vozilo, ukoliko je slobodno:

      ![agent-2-8](https://github.com/user-attachments/assets/68c4b1e5-3a0e-4c49-ae73-4ca398052404)

    - Zatim biranje klijenta kao i dodatnih opcija:

      ![agent-2-9](https://github.com/user-attachments/assets/cfc809a7-bd0a-4e30-b469-6535707edf22)

    - Za kraj pregled, prije samog kreiranja:

      ![agent-2-10](https://github.com/user-attachments/assets/4e01372f-3752-4d21-aab1-f0a16765f392)

    *Novokreirano iznajmljivanje*

    ![agent-2-11](https://github.com/user-attachments/assets/e525a4bb-eb21-4e4b-9a95-a5927403d61c)
  

### Rad sa klijentima

![agent-1-1](https://github.com/user-attachments/assets/4c2e4d28-bcdb-492b-bc8d-48af66d37e90)

Agenti mogu da:
- Dodaju nove klijente u sistem
  
![agent-1-2](https://github.com/user-attachments/assets/7d1af35d-745c-4738-a8e4-cef5bfb7f92d)

*Poruka o uspješnosti*

![agent-1-3](https://github.com/user-attachments/assets/768a5cf1-3337-41d8-9c8a-f3dddeb586b0)

- Uređuju podatke postojećih klijenata

![agent-1-4](https://github.com/user-attachments/assets/17f08d56-83b7-4d2e-8086-e219eda2e795)

*Poruka ukoliko bi uneseni podaci bili nevalidni*

![agent-1-5](https://github.com/user-attachments/assets/153fb9bc-0461-4b73-a482-3a26ed1fb769)
  
- Pretražuju bazu klijenata (omogućena tekstualna pretraga po svim relevantnim poljima, kao i sortiranje po određenim kolonama), te iz tog pregleda mogu takođe da obrišu određenog klijenta

![agent-1-6](https://github.com/user-attachments/assets/2dff3ce6-ea40-4cbf-bcf0-583a68db70f0)

*Poruka o uspješnosti brisanja klijenta*

![agent-1-7](https://github.com/user-attachments/assets/b0b1f087-9552-458e-9385-f6f4e588ac86)


### Pregled vozila

![agent-3-1](https://github.com/user-attachments/assets/70d9a46e-31c6-45a1-ac6b-d9994092cc57)

Interfejs pruža jednostavan način za:
- Pretragu vozila (tekstualnom pretragom ili izborom odgovarajućeg filtera)

![agent-3-2](https://github.com/user-attachments/assets/02aa218a-2c01-427d-8180-d8c0e6f7e281)

- Kreiranje iznajmljivanja za odabrano vozilo

![agent-3-3](https://github.com/user-attachments/assets/88328379-71d6-4052-8175-73273d256910)

*Poruka o uspješnosti*

![agent-3-4](https://github.com/user-attachments/assets/fc7bb421-db42-49de-a783-6a33e99f1bcc)


### Pregled problema kao i njihovih detalja

![agent-4-1](https://github.com/user-attachments/assets/25fb11fb-4753-4a7e-8b84-d067bcd45475)

![agent-4-2](https://github.com/user-attachments/assets/0e2cd9e0-0e15-401b-a082-234c44ef7ce3)


### Podešavanja naloga

![agent-5-1](https://github.com/user-attachments/assets/96cdab46-f8c6-426d-975b-78c8b82c0e6a)

*Korisnik može da vidi svoje lične podatke, te isto tako da određene ažurira, kao i da promjeni svoju lozinku.*


---

## Administratorski interfejs

Nakon prijave sa administratorskim nalogom, korisniku se otvara prozor na kome je prikazana prva stranica - *Upravljanje agentima*

![admin-1-1](https://github.com/user-attachments/assets/ea90de7d-d8c9-4428-9644-5e1995127cdf)

*CRUD funkcionalnosti slične kao i u ostatku sistema...*


### Pregled i obrada problema prijavljenih od strane agenata

![admin-2-1](https://github.com/user-attachments/assets/4f82e95d-cf2d-429d-b6b7-f69970675c55)

![admin-2-2](https://github.com/user-attachments/assets/9a91844f-847a-4555-a19b-c0b27556bf9e)

![admin-2-3](https://github.com/user-attachments/assets/8cb9a87e-3a26-41a9-8ec7-7a7b11c50d6b)


### Pregled iznajmljivanja

![admin-3-1](https://github.com/user-attachments/assets/060e2fba-86c0-4a35-b751-8e532586af1f)

- Pretraga i pregled detalja iznajmljivanja:
  
  ![admin-3-2](https://github.com/user-attachments/assets/71fc35f4-feba-4eb6-bb15-c18fcb4f626c)
  
  ![admin-3-3](https://github.com/user-attachments/assets/fd24eae1-90bc-487c-85a4-50eaa9a1f8cf)
  

### Upravljanje voznim parkom

![admin-4-1](https://github.com/user-attachments/assets/a0f1aff5-0ddf-452f-9180-c18bd05feea2)

Administratori mogu da dodaju nova vozila u sistem, uređuju postojeća ili da ih brišu:

![admin-4-2](https://github.com/user-attachments/assets/3d871537-55a6-4b95-80e5-2e1523187859)
*Dodavanje vozila*

![admin-4-3](https://github.com/user-attachments/assets/d4108fc7-bd86-4afe-9445-b06afa130387)
*Brisanje vozila*

![admin-4-4](https://github.com/user-attachments/assets/e00899b2-9044-46bb-8d0c-e6a5a580d0af)
*Izmjena vozila*


### Kreiranje i upravljanje dodatnim opcijama iznajmljivanja

![admin-5-1](https://github.com/user-attachments/assets/a5c93b48-8fac-49dd-8352-73ecf5261185)

![admin-5-2](https://github.com/user-attachments/assets/26477707-ead2-4e1c-be71-4d02e6840b9c)


### Podešavanja naloga

![admin-6](https://github.com/user-attachments/assets/792798b3-5977-46fb-993c-ae4c52916261)

*Korisnik može da vidi svoje lične podatke, te isto tako da određene ažurira, kao i da promjeni svoju lozinku.*


## Podešavanja aplikacije

Svi korisnici mogu prilagoditi određene aspekte korisničkog interfejsa prema svojim preferencijama:
- Promjena teme

![theme-selection](https://github.com/user-attachments/assets/62301db8-fb9f-44d8-a127-7279bc045984)

Login stranice za različite teme:

![login-dark](https://github.com/user-attachments/assets/2f4301d3-475c-4c24-a332-cf95dd1a5cb8)

![login-light](https://github.com/user-attachments/assets/34266efb-7474-45cc-9a52-398047435e59)

![login-sea](https://github.com/user-attachments/assets/ced2c250-b418-4cd9-a9e5-44e5be97f6d1)


- Promjena jezika 

![language-selection](https://github.com/user-attachments/assets/e51793a9-3810-423f-8433-7ae8732e3f0e)


## Odjava sa sistema

Odjava se vrši klikom na dugme koje se nalazi u gornjem desnom uglu prozora aplikacije.

![logout](https://github.com/user-attachments/assets/caccd92e-035f-4bbe-8d45-a651bd386798)

*Korisnik mora da potvrdi odjavu*

![logout-confirm](https://github.com/user-attachments/assets/c1cabe34-144e-4e8a-80e9-60abdb408148)
