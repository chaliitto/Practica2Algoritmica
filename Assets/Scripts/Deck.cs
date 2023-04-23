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
    public Text probMessage;
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
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
        }
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
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
        Puntos.text = "Puntos: " + player.GetComponent<CardHand>().points;

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */

        if (player.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "HAS PERDIDO";
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
        MensajePuntosDealer.text = "Dealer: " + PuntosDealer + " puntos";

        if (PuntosDealer == 21)
        {
            finalMessage.text = "El dealer hizo BlackJack. Has perdido";
        }
        else if (PuntosJuagador == 21)
        {
            finalMessage.text = "Enhorabuena, has hecho BlackJack. Has ganado";
        }
        else if (PuntosDealer > 21)
        {
            finalMessage.text = "La banca se ha pasado, has ganado";
        }
        else if (PuntosDealer == PuntosJuagador)
        {
            finalMessage.text = "Habeis tenido un empate";
        }
        else
        {
            finalMessage.text = "La banca te ha superado, has perdido";
        }


    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }

}
