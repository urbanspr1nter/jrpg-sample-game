using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jrpg.InventorySystem;
using Jrpg.InventorySystem.Items;
using Jrpg.InventorySystem.PgItems;

public class Treasure : MonoBehaviour, IItemSubscriber
{
    private bool collided;
    private Item item;

    private void Acquire()
    {
        var baseItem = new BaseItem(item);
        baseItem.Register(this);

        MainGame.Party().ReceiveItem(baseItem, ItemReceiveAction.Treasure);
        MainGame.Log($"Received: {baseItem.Name}.");

        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        collided = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        collided = false;
    }

    void Update()
    {
        if (collided && Input.GetKey(KeyCode.Space))
        {
            GetComponent<AudioSource>().Play();

            item = MainGame
                .ItemGenerator()
                .GenerateItem(
                    MainGame.DropSourcesDb()["Treasure"]
                        .Find(t => t.Name.Equals(gameObject.tag)
                    )
                );

            Acquire();
        }
    }

    public void Publish(Dictionary<string, object> message)
    {
        MainGame.Log($"{message["MemberName"]} {message["Message"]}");
    }
}
