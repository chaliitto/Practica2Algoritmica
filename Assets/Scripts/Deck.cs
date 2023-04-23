using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probDealermasJugador;
    public Text prob1721;
    public Text probmayor21;
    public Text Puntos;
    public Text MensajePuntosDealer;

    public int[] values = new int[52];
    int cardIndex = 0;

    private void Awake()
    {
        InitCardValues();

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */

        int cardIndex = 0;
        for (int i = 0; i < 4; i++) //4 palos: corazones, diamantes, picas, tréboles
        {
            for (int j = 1; j <= 13; j++) //13 cartas por palo
            {
                values[cardIndex] = j <= 10 ? j : 10; //Asigna el valor de la carta (1-10) o 10 para las figuras

                if (j == 1)
                {
                    values[cardIndex] = 11;
                }
                else if (j <= 10)
                {
                    values[cardIndex] = j;
                }
                else
                {
                    values[cardIndex] = 10;
                }
                cardIndex++;
            }
        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */
        for (int i = 0; i < values.Length; i++)
        {
            var newPos = Random.Range(0, 52);
            var pos_actual = values[i];
            var face_actual = faces[i];

            faces[i] = faces[newPos];
            faces[newPos] = face_actual;

            values[i] = values[newPos];
            values[newPos] = pos_actual;
        }
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        { 
            PushPlayer();
            PushDealer();
            Puntos.text = player.GetComponent<CardHand>().points.ToString();
            MensajePuntosDealer.text = "";

            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */

            if (player.GetComponent<CardHand>().points == 21 || dealer.GetComponent<CardHand>().points == 21)
            {
                Stand();
            }
        }
    }

    private void CalculateProbabilities()
    {
        int PuntosJugador = player.GetComponent<CardHand>().points;

        //Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador


        // - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta

        int si = 0;
        int no= 0;

        for (int i = cardIndex; i < values.Length; i++)
        {
            if (PuntosJugador + values[i] >= 17 && PuntosJugador + values[i] <= 21)
            {
                si++;
            }
            else
            {
                no++;
            }
        }
        int cartasTotales = si + no;
        double probabilidad = ((double)si * 100) / (double)cartasTotales;
        prob1721.text = probabilidad.ToString("F2") + "%";


        //- Probabilidad de que el jugador obtenga más de 21 si pide una carta
        //Teniendo en cuenta la los puntos del jugador, probamos a sumarle los valores de cada carta de la baraja. 
        // Luego vemos cual de los resultados se pasa de 21, y con ello calculamos el porcentaje

        int si2 = 0;
        int no2 = 0;

        for (int i = cardIndex; i < values.Length; i++)
        {
            if (PuntosJugador + values[i] > 21)
            {
                si2++;
            }
            else
            {
                no2++;
            }
        }
        int cartasTotales2 = si2 + no2;
        double probabilidad2 = ((double)si2 * 100) / (double)cartasTotales2;
        probmayor21.text = probabilidad2.ToString("F2") + "%";
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
        CalculateProbabilities();
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        //Repartimos carta al jugador
        PushPlayer();
        Puntos.text = player.GetComponent<CardHand>().points.ToString();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */

        if (player.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "Te has pasado de puntos, HAS PERDIDO :(";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
        hitButton.interactable = false;
        stickButton.interactable = false;

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */

        while (dealer.GetComponent<CardHand>().points < 17)
        {
            PushDealer();
        }

        int PuntosDealer = dealer.GetComponent<CardHand>().points;
        int PuntosJuagador = player.GetComponent<CardHand>().points;
        MensajePuntosDealer.text = PuntosDealer.ToString();

        if (PuntosDealer == 21)
        {
            finalMessage.text = "BLACKJACK DEL DEALER, HAS PERDIDO :(";
        }
        else if (PuntosJuagador == 21)
        {
            finalMessage.text = "BLACKJACK, HAS GANADO!!!";
        }
        else if (PuntosDealer > 21)
        {
            finalMessage.text = "El dealer se ha pasado de puntos, HAS GANADO!!!";
        }
        else if (PuntosDealer == PuntosJuagador)
        {
            finalMessage.text = "EMPATE :|";
        } 
        else if(PuntosDealer <= PuntosJuagador)
        {
            finalMessage.text = "Has hecho mas puntos que la banca, HAS GANADO!!!";
        }
        else
        {
            finalMessage.text = "Has hecho menos puntos que la banca, HAS PERDIDO :(";
        }


    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        Puntos.text = "";
        MensajePuntosDealer.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }

}
