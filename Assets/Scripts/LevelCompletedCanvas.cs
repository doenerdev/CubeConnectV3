using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelCompletedCanvas : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _ratingStarsContainer;
    [SerializeField] private GameObject _ratingStarPrefab;
    [SerializeField] private GameObject _noRatingStarPrefab;
    [SerializeField] private Text _qtyRedosText;
    [SerializeField] private Text _qtyUnfoldsText;

    public Text errorText;

    public void PlayLevelCompleteAnimationIn(int qtyRedos, int qtyUnfolds, uint rating)
    {
        Debug.Log("PlayLevel Completed");
        _qtyRedosText.text = qtyRedos.ToString();
        _qtyUnfoldsText.text = qtyUnfolds.ToString();

        foreach (Transform child in _ratingStarsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < rating; i++)
        {
            GameObject ratingStar = Instantiate(_ratingStarPrefab);
            ratingStar.transform.SetParent(_ratingStarsContainer.transform);
        }
        for (uint i = rating; i < 3; i++)
        {
            GameObject ratingStar = Instantiate(_noRatingStarPrefab);
            ratingStar.transform.SetParent(_ratingStarsContainer.transform);
        }

        _animator.SetTrigger("AnimIn");
    }

    public void PlayLevelCompleteAnimationOut()
    {
        _animator.SetTrigger("AnimOut");
    }
}
