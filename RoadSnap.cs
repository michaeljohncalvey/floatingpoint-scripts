﻿using System.Collections;
using System.Collections.Generic;
using VRTK;
using UnityEngine;

public class RoadSnap : MonoBehaviour {

	public bool manualUse;
	public bool setBuildingPos;

	VRTK_InteractableObject interact;
	bool objectUsed;
	Renderer rend;
	Collider[] hitColliders;
	GameObject nearestBuilding;
	MeshRenderer targetRend;
	MeshRenderer thisRend;
	Vector3 targetPosition;
	float distanceToMoveX;
	float distanceToMoveZ;
	float spacing;
	float frontTargetPoint;
	float frontThisPoint;
	float pointDifference;
	int buildingLayer = 8;
	int layerMask;

	void Update() {
		if (objectUsed || manualUse) {
			gameObject.layer = 9;
			getNearbyBuildings ();
		} 
		else {
			gameObject.layer = buildingLayer;
		}

		if (setBuildingPos) {
			checkForNearbyBuilding();
		} 
	}

	void Start ()
	{
		layerMask = 1 << buildingLayer;
		rend = GetComponent<Renderer>();
		// Adds listeners for controller grab to both controllers
		GameObject.Find("RightController").GetComponent<VRTK_ControllerEvents>().AliasGrabOn+=
			new ControllerInteractionEventHandler(DoGrabStart);
		GameObject.Find("LeftController").GetComponent<VRTK_ControllerEvents>().AliasGrabOn +=
			new ControllerInteractionEventHandler(DoGrabStart);

		// Add listeners for controller release to both controllers
		GameObject.Find("RightController").GetComponent<VRTK_ControllerEvents>().AliasGrabOff +=
			new ControllerInteractionEventHandler(DoGrabRelease);
		GameObject.Find("LeftController").GetComponent<VRTK_ControllerEvents>().AliasGrabOff +=
			new ControllerInteractionEventHandler(DoGrabRelease);

		interact = gameObject.GetComponent<VRTK_InteractableObject>();
		objectUsed = false;
	}

	void DoGrabRelease(object sender, ControllerInteractionEventArgs e)
	// Grab end event listener
	{
		if (objectUsed == true) {
			checkForNearbyBuilding ();
			objectUsed = false;
		} 
	}

	void DoGrabStart(object sender, ControllerInteractionEventArgs e)
	// Grab start event listener
	{
		if(interact.IsGrabbed() == true) {	
			objectUsed = true;
		}
	}

	void checkForNearbyBuilding() {
		if (nearestBuilding) {
			gameObject.GetComponent<BoxCollider> ().enabled = false;
			setPosition ();
			gameObject.GetComponent<BoxCollider> ().enabled = true;
			setBuildingPos = false;
		} else {
			// Debug.Log (nearestBuilding);
			GetComponent<BoxCollider> ().enabled = true;
		}
	}

	void getNearbyBuildings() {
		hitColliders = Physics.OverlapSphere (transform.position, 1.5f, layerMask);
		if (hitColliders.Length == 0) {
			nearestBuilding = null;
		} 
		else {
			foreach (Collider hitcol in hitColliders) {
				if (hitcol.CompareTag ("residential") && hitcol != GetComponent<Collider> ()) {
					nearestBuilding = hitcol.gameObject;
					Debug.Log ("FOUND HIT: " + nearestBuilding);

					if (Mathf.Abs ((nearestBuilding.transform.position.x - transform.position.x)) < Mathf.Abs ((nearestBuilding.transform.position.z - transform.position.z))) {
						// Debug.Log ("Closer to z");
					}

					if (Mathf.Abs ((nearestBuilding.transform.position.x - transform.position.x)) > Mathf.Abs ((nearestBuilding.transform.position.z - transform.position.z))) {
						// Debug.Log ("Closer to x");
					}

				} 
			}
		}
	}

	void setPosition() {
		targetRend = nearestBuilding.GetComponent<MeshRenderer>();
		thisRend = gameObject.GetComponent<MeshRenderer> ();

		targetPosition = nearestBuilding.transform.position;
		spacing = 0.1f;

		distanceToMoveX = (targetRend.bounds.size.x / 2) + (thisRend.bounds.size.x / 2) + spacing;
		distanceToMoveZ = (targetRend.bounds.size.z / 2) + (thisRend.bounds.size.z / 2) + spacing;

		frontTargetPoint = nearestBuilding.transform.position.x + (targetRend.bounds.size.x / 2);
		frontThisPoint = transform.position.x -(thisRend.bounds.size.x / 2);
		pointDifference = Mathf.Abs (frontThisPoint) - Mathf.Abs (frontTargetPoint);


		if (Mathf.Abs((nearestBuilding.transform.position.x - transform.position.x)) > Mathf.Abs((nearestBuilding.transform.position.z - transform.position.z))) {
			snapToX ();
		}


		else if (Mathf.Abs ((nearestBuilding.transform.position.x - transform.position.x)) < Mathf.Abs ((nearestBuilding.transform.position.z - transform.position.z))) {
			snapToZ ();
		}

		transform.parent = null;

		Debug.Log ("IT RAN: " + transform.position);

		// set z position, and align x axis
		//transform.position = new Vector3(transform.position.x + pointDifference, targetPosition.y, targetPosition.z - distanceToMoveZ);

	}

	void snapToX() {
		if (transform.position.x < nearestBuilding.transform.position.x) {
			// Debug.Log ("Called snap position x: 1");

			transform.parent = nearestBuilding.transform;
			transform.localPosition = new Vector3 (0, 0, transform.localPosition.z);
		} 

		else {
			// Debug.Log ("Called snap position x: 2");

			transform.parent = nearestBuilding.transform;
			transform.localPosition = new Vector3 (0, 0, transform.localPosition.z);
		}
	}

	void snapToZ() {
		if (transform.position.z < nearestBuilding.transform.position.z) {
			// Debug.Log ("Called snap position Z: 1");

			transform.parent = nearestBuilding.transform;
			transform.localPosition = new Vector3 (transform.localPosition.x, 0, 0);
		} else {
			// Debug.Log ("Called snap position Z: 2");

			transform.parent = nearestBuilding.transform;
			transform.localPosition = new Vector3 (transform.localPosition.x, 0, 0);
		}
	}

}