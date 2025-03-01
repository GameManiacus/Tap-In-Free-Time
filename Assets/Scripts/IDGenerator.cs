using UnityEngine;

public static class IDGenerator
{
    public static string GenerateID()
    {
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string numbers = "0123456789";

        string firstTwo = "" + letters[Random.Range(0, letters.Length)]
                            + letters[Random.Range(0, letters.Length)];


        string end = "" + letters[Random.Range(0, letters.Length)];

        string firstSetNumbers = "";

        for (int i = 0; i < 4; i++)
        {
            firstSetNumbers += numbers[Random.Range(0, numbers.Length)];
        }

        return firstTwo + firstSetNumbers + end;
    }

}
