using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestiaryController : MonoBehaviour
{
    [SerializeField] private Sprite[] characters;
    [SerializeField] private Image image;
    [SerializeField] private Animator pageAnimator;
    [SerializeField] private Animator imageAnimator;

    private int currentPage = 0;
    private bool isAnimating = false;

    private void Start()
    {
        image.sprite = characters[currentPage];
    }

    public void NextPage()
    {
        if (currentPage >= characters.Length - 1 || isAnimating)
            return;

        StartCoroutine(PageChange(currentPage + 1, "Next"));
    }

    public void PreviousPage()
    {
        if (currentPage <= 0 || isAnimating)
            return;

        StartCoroutine(PageChange(currentPage - 1, "Previous"));
    }

    private IEnumerator PageChange(int newPageIndex, string pageAnimationTrigger)
    {
        isAnimating = true;

        imageAnimator.SetTrigger("out");

        // Wait until the "out" animation has fully played
        yield return new WaitUntil(() => imageAnimator.GetCurrentAnimatorStateInfo(0).IsName("out"));
        yield return new WaitForSeconds(imageAnimator.GetCurrentAnimatorStateInfo(0).length);

        pageAnimator.SetTrigger(pageAnimationTrigger);

        yield return new WaitUntil(() => pageAnimator.GetCurrentAnimatorStateInfo(0).IsName(pageAnimationTrigger));
        yield return new WaitForSeconds(pageAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Now change sprite and trigger page animation
        image.sprite = characters[newPageIndex];

        // Then trigger "in" animation
        imageAnimator.SetTrigger("in");

        currentPage = newPageIndex;
        isAnimating = false;
    }
}
