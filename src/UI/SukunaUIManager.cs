using UnityEngine;
using UnityEngine.UI;

namespace SukunaMod.UI
{
    public class SukunaUIManager : MonoBehaviour
    {
        private Canvas canvas;
        private CanvasScaler canvasScaler;
        
        // Button references
        private Button transformButton;
        private Button cleaveButton;
        private Button slashButton;
        private Button domainButton;

        // Button text displays
        private Text transformText;
        private Text cleaveText;
        private Text slashText;
        private Text domainText;

        // Cooldown displays
        private Image cleaveCooldownImage;
        private Image slashCooldownImage;
        private Image domainCooldownImage;

        private SukunaPlayer currentPlayer;

        public void Initialize(SukunaPlayer player)
        {
            currentPlayer = player;
            CreateUI();
        }

        private void CreateUI()
        {
            // Create Canvas if it doesn't exist
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("SukunaCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                
                canvasScaler = canvasObj.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

                // Create GraphicRaycaster for interactivity
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // Create Transform Button
            transformButton = CreateButton("Transform", new Vector2(50, 50), new Color(0.8f, 0.2f, 0.2f, 0.8f));
            transformButton.onClick.AddListener(() => OnTransformButtonClicked());
            transformText = transformButton.GetComponentInChildren<Text>();

            // Create Cleave Button
            cleaveButton = CreateButton("Cleave [C]", new Vector2(50, 150), new Color(0.2f, 0.2f, 0.8f, 0.8f));
            cleaveButton.onClick.AddListener(() => OnCleaveButtonClicked());
            cleaveText = cleaveButton.GetComponentInChildren<Text>();
            cleaveCooldownImage = cleaveButton.image;

            // Create Slash Button
            slashButton = CreateButton("Slash [V]", new Vector2(50, 250), new Color(0.2f, 0.8f, 0.2f, 0.8f));
            slashButton.onClick.AddListener(() => OnSlashButtonClicked());
            slashText = slashButton.GetComponentInChildren<Text>();
            slashCooldownImage = slashButton.image;

            // Create Domain Button
            domainButton = CreateButton("Domain [X]", new Vector2(50, 350), new Color(0.8f, 0.8f, 0.2f, 0.8f));
            domainButton.onClick.AddListener(() => OnDomainButtonClicked());
            domainText = domainButton.GetComponentInChildren<Text>();
            domainCooldownImage = domainButton.image;

            SukunaMod.Logger.LogInfo("Sukuna UI Buttons Created!");
        }

        private Button CreateButton(string labelText, Vector2 position, Color backgroundColor)
        {
            // Create button GameObject
            GameObject buttonObj = new GameObject($"Button_{labelText}");
            RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.SetParent(canvas.transform, false);
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(120, 80);

            // Add Image component (background)
            Image image = buttonObj.AddComponent<Image>();
            image.color = backgroundColor;

            // Add Button component
            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = image;

            // Add ColorTween for button interaction
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = backgroundColor;
            colorBlock.highlightedColor = new Color(backgroundColor.r + 0.2f, backgroundColor.g + 0.2f, backgroundColor.b + 0.2f, 0.9f);
            colorBlock.pressedColor = new Color(backgroundColor.r - 0.1f, backgroundColor.g - 0.1f, backgroundColor.b - 0.1f, 0.9f);
            colorBlock.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            button.colors = colorBlock;

            // Create Text child
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = new Vector2(120, 80);

            Text text = textObj.AddComponent<Text>();
            text.text = labelText;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 14;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            return button;
        }

        private void OnTransformButtonClicked()
        {
            if (currentPlayer == null)
                return;

            currentPlayer.ToggleTransformation();
            SukunaRPC.SendTransformRPC(PlayerControl.LocalPlayer.PlayerId, currentPlayer.IsSukuna);
            
            transformText.text = currentPlayer.IsSukuna ? "Sukuna ON [S]" : "Transform [S]";
            SukunaMod.Logger.LogInfo($"Transform button clicked! IsSukuna: {currentPlayer.IsSukuna}");
        }

        private void OnCleaveButtonClicked()
        {
            if (currentPlayer == null || !currentPlayer.CanUseAbility("Cleave"))
                return;

            Vector3 playerPos = PlayerControl.LocalPlayer.transform.position;
            currentPlayer.StartAbilityCooldown("Cleave");

            // Kill all nearby players
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer || player.Data.IsDead)
                    continue;

                float distance = Vector3.Distance(player.transform.position, playerPos);
                if (distance <= 3.0f)
                {
                    player.Exiled();
                }
            }

            SukunaRPC.SendCleaveRPC(PlayerControl.LocalPlayer.PlayerId, playerPos, 3.0f);
            SukunaMod.Logger.LogInfo("Cleave button clicked!");
        }

        private void OnSlashButtonClicked()
        {
            if (currentPlayer == null || !currentPlayer.CanUseAbility("Slash"))
                return;

            Vector3 playerPos = PlayerControl.LocalPlayer.transform.position;
            PlayerControl closestPlayer = null;
            float closestDistance = 2.5f;

            // Find closest target
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer || player.Data.IsDead)
                    continue;

                float distance = Vector3.Distance(player.transform.position, playerPos);
                if (distance < closestDistance)
                {
                    closestPlayer = player;
                    closestDistance = distance;
                }
            }

            if (closestPlayer != null)
            {
                currentPlayer.StartAbilityCooldown("Slash");
                closestPlayer.Exiled();
                SukunaRPC.SendSlashRPC(PlayerControl.LocalPlayer.PlayerId, closestPlayer.PlayerId);
                SukunaMod.Logger.LogInfo("Slash button clicked!");
            }
        }

        private void OnDomainButtonClicked()
        {
            if (currentPlayer == null || !currentPlayer.CanUseAbility("Domain"))
                return;

            currentPlayer.StartAbilityCooldown("Domain");
            currentPlayer.ActivateDomain(10f, 5.0f);

            Vector3 playerPos = PlayerControl.LocalPlayer.transform.position;
            SukunaRPC.SendDomainRPC(PlayerControl.LocalPlayer.PlayerId, playerPos, 5.0f, 10f);
            SukunaMod.Logger.LogInfo("Domain Expansion button clicked!");
        }

        private void Update()
        {
            if (currentPlayer == null)
                return;

            // Update cooldown displays
            UpdateCooldownDisplay(cleaveButton, cleaveText, "Cleave");
            UpdateCooldownDisplay(slashButton, slashText, "Slash");
            UpdateCooldownDisplay(domainButton, domainText, "Domain");

            // Hide buttons if not Sukuna
            if (!currentPlayer.IsSukuna)
            {
                cleaveButton.gameObject.SetActive(false);
                slashButton.gameObject.SetActive(false);
                domainButton.gameObject.SetActive(false);
            }
            else
            {
                cleaveButton.gameObject.SetActive(true);
                slashButton.gameObject.SetActive(true);
                domainButton.gameObject.SetActive(true);
            }
        }

        private void UpdateCooldownDisplay(Button button, Text text, string abilityName)
        {
            float cooldown = currentPlayer.GetAbilityCooldown(abilityName);
            
            if (cooldown > 0)
            {
                text.text = $"{abilityName}\n{cooldown:F1}s";
                button.interactable = false;
                
                // Darken button during cooldown
                Color color = button.colors.normalColor;
                color.a = 0.5f;
                button.image.color = color;
            }
            else
            {
                button.interactable = true;
                button.image.color = button.colors.normalColor;
            }
        }

        public void Hide()
        {
            if (canvas != null)
                canvas.gameObject.SetActive(false);
        }

        public void Show()
        {
            if (canvas != null)
                canvas.gameObject.SetActive(true);
        }
    }
}
