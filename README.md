<h1>Twitch Client</h1>

<p>
Ein kleines Anwendungsprogramm um sich mit einem Twitch Chat zu verbinden und automatisch nach einem festgelegtem Intervall
Nachrichten in den Chat schreiben zu lassen. 
</p>
<p>
Um sich anzumelden muss der Anwender seinen Twitch Login-Namen eingeben, sein OAuth Code (https://twitchapps.com/tmi/) eingeben 
und den entsprechenden Channel, welchen er betreten möchte. Anschließend kann der Anwender entweder einen Zeitinterval in Sekunden angeben
und den Text der ausgegeben werden soll oder kann eine Nachricht sofort verschicken.<br>
</p>
<p>
Mittels Connect bzw Disconnect betritt bzw. verläßt der Anwender den Channel. 
Fehler werden entsprechend ausgegeben.
</p>
<p>
Info zum OAuth:<br>
Über den <a href="https://twitchapps.com/tmi/">Link</a> muss der Client für den Benutzer Account frei gegeben werden. Der Code wird dann ohne
"oauth:" in das entsprechende Feld hinein kopiert.
</p>
<h2>Notation</h2>
<p>
Die XAML-Elemente wurden wie folgt benannt:
</p>
<ul>
  <li>TextBoxen -> tb..</li>
  <li>Labels -> lb..</li>
  <li>Buttons -> bt..</li>
  <li>Textblock -> tblock..</li>
</ul>
<p>
Im Code selber muss für jeden selbst noch seine entsprechende ClientID gesetzt werden. Die Zeile ist im Code auskommentiert.
</p>
