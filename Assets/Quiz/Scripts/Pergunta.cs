using System.Collections.Generic;

public enum TipoMidia { Texto, Imagem, Audio, Video }

[System.Serializable]
public class Pergunta
{
    public string texto = "";
    public TipoMidia tipoMidia = TipoMidia.Texto;
    public string caminhoMidia = "";
    public List<string> alternativas = new List<string>();
    public int indiceCorreto = 0;
}