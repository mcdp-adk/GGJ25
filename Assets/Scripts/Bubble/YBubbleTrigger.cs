using System.Collections;
using UnityEngine;

public class YBubbleTrigger : MonoBehaviour
{
    public YBubbleManager manager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = GameManager.Instance.GetPlayer().GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.StartCoroutine(ApplyYBuff(playerController));
            }
            manager.OnBubbleCollected();
        }
    }

    private IEnumerator ScaleOverTime(Transform target, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            target.localScale = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.localScale = to;
    }

    private IEnumerator ApplyYBuff(PlayerController player)
    {
        if (player == null) yield break;
        if (player.jumpBuffMultiplier != 1f) yield break; // 防止重复buff
        player.jumpBuffMultiplier = 3f;
        player.speedBuffMultiplier = 3f;
        player.IsImmuneToSpikes = true;
        yield return new WaitForSeconds(10f);
        player.jumpBuffMultiplier = 1f;
        player.speedBuffMultiplier = 1f;
        player.IsImmuneToSpikes = false;
    }
}