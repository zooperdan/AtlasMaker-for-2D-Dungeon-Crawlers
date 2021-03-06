# Export binary format

> Note: All data types are unsigned.

## Header

| Type | Size | Description |
| :------------- |  :------------- |  :------------- |
| Byte | 1 | Dungeon depth  |
| Byte | 1  | Dungeon width |
| Int16 | 1 | Screen width |
| Int16 | 1 | Screen height |
| Byte | 1  | Layer count |

> Repeating layer data from this point based on layer count byte value.

### Layers

| Type | Size | Description |
| :------------- |  :------------- |  :------------- |
| Byte | 1 | Layer ID |
| Byte | 1  | Layer Name Length |
| Bytes | Depends on previous byte value | Layer Name |
| Byte | 1 | Render mode |
| Byte | 1  | Layer Type Length |
| Bytes | Depends on previous byte value | Layer Type |
| Byte | 1 | Tiles count |

#### Tiles

> Repeating tile data from this point based on tiles count byte value.
> 
| Type | Size | Description |
| :------------- |  :------------- |  :------------- |
| Byte | 1  | Tile Type Length |
| Bytes | Depends on previous byte value | Tile Type |
| Byte | 1 | Map X |
| Byte | 1  | Map Y |
| Int16 | 1 | Screen X |
| Int16 | 1 | Screen Y |
| Int16 | 1 | Atlas Coords X |
| Int16 | 1 | Atlas Coords Y |
| Int16 | 1 | Atlas Coords Width |
| Int16 | 1 | Atlas Coords Height |

