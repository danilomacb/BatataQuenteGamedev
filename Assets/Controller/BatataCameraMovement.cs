using UnityEngine;
using UnityEngine.InputSystem;

public class BatataCameraMovement : MonoBehaviour
{
    [Header("Alvo da Câmera")]
    public Transform alvo;
    public float alturaDoFoco = 1.0f;

    [Header("As 2 Ações do Input System")]
    public InputActionReference acaoZoom;
    public InputActionReference acaoOrbitar;

    [Header("Configurações de Distância (Zoom)")]
    public float distanciaAtual = 5f;
    public float distanciaMinima = 2f;
    public float distanciaMaxima = 15f;
    public float velocidadeZoom = 0.01f;

    [Header("Configurações de Rotação")]
    public float velocidadeRotacao = 0.5f;

    [Header("Limites de Rotação Vertical (Cima/Baixo)")]
    public float limiteMinimoY = -20f; // O quanto a câmera pode descer (olhar para cima)
    public float limiteMaximoY = 80f;  // O quanto a câmera pode subir (olhar para baixo)

    private float anguloX = 0f;
    private float anguloY = 0f;

    void Start()
    {
        // Guarda a rotação inicial para evitar pulos na câmera
        Vector3 angulos = transform.eulerAngles;
        anguloX = angulos.y;
        anguloY = angulos.x;

        // Ativa apenas as duas ações necessárias
        if (acaoZoom != null) acaoZoom.action.Enable();
        if (acaoOrbitar != null) acaoOrbitar.action.Enable();
    }

    void LateUpdate()
    {
        if (alvo == null) return;

        // 1. LÓGICA DO ZOOM
        float scroll = acaoZoom.action.ReadValue<Vector2>().y;
        if (scroll != 0)
        {
            distanciaAtual -= scroll * velocidadeZoom;
            distanciaAtual = Mathf.Clamp(distanciaAtual, distanciaMinima, distanciaMaxima);
        }

        // 2. LÓGICA DE ROTAÇÃO
        // O valor aqui já vem formatado: só terá números se o botão direito estiver estritamente segurado
        Vector2 movimentoMouse = acaoOrbitar.action.ReadValue<Vector2>();

        anguloX += movimentoMouse.x * velocidadeRotacao;
        anguloY -= movimentoMouse.y * velocidadeRotacao;

        anguloY = Mathf.Clamp(anguloY, limiteMinimoY, limiteMaximoY);

        // 3. CÁLCULO DA POSIÇÃO FINAL NO ESPAÇO 3D
        Vector3 posicaoFoco = alvo.position + (Vector3.up * alturaDoFoco);
        Quaternion rotacao = Quaternion.Euler(anguloY, anguloX, 0);
        Vector3 posicaoFinal = posicaoFoco + (rotacao * new Vector3(0, 0, -distanciaAtual));

        transform.position = posicaoFinal;
        transform.LookAt(posicaoFoco);
    }
}