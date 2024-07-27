using System;
using System.Collections.Generic;
using System.Text;

namespace Algorithm
{
	class Pos
	{
		public Pos(int y, int x) { Y = y; X = x; }
		public int Y;
		public int X;
	}

	class Player
	{
		public int PosY { get; private set; }
		public int PosX { get; private set; }
		Random _random = new Random();
		Board _board;

		enum Dir
		{
			Up = 0,
			Left = 1,
			Down = 2,
			Right = 3 
		}

		int _dir = (int)Dir.Up;
		List<Pos> _points = new List<Pos>();

		public void Initialize(int posY, int posX, Board board)
        {
            PosY = posY;
            PosX = posX;
            _board = board;

            //RightHand(); 우수법 오른손의 법칙으로 
            //BFS();
            AStar();
            
        }

        struct PQNode : IComparable<PQNode>
        {
            public int F;
            public int G;
            public int Y;
            public int X;

            public int CompareTo(PQNode other)
            {
                if(F == other.F)
                    return 0;
                return F < other.F ? 1 : -1;
                throw new NotImplementedException();
            }
        }

        void AStar()
        {
            int[] deltaY = new int[] { -1, 0, 1, 0 };
            int[] deltaX = new int[] { 0, -1, 0, 1 };
            int[] cost = new int[] {1, 1, 1, 1 };   

            // 점수 매기기 
            // F = G + H
            // F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐) 
            // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
            // H = 목적지에 얼마나 가까운지 (작을 수록 좋음, 고정) 

            //(y,x) 이미 방문했는지 여부 (방문 = closed 상태)
            bool[,] closed = new bool[_board.Size,_board.Size]; //CloseList

            //(y,x) 가는 길을 한 번이라도 발견했는지 
            // 발견X => MaxValue
            // 발견O => F = G + H
            int[,] open = new int[_board.Size, _board.Size]; //OpenList

            for(int y =  0; y < _board.Size; y++) 
                for(int x = 0; x < _board.Size; x++)
                    open[y, x] = Int32.MaxValue;

            Pos[,] parent = new Pos[_board.Size, _board.Size];

            // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구 
            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>(); 

            // 시작점 발견 (예약 진행) 시작 점이기에 G = 0, H만 계산 
            open[PosY,PosX] = Math.Abs(_board.DestY - PosY) + Math.Abs(_board.DestX - PosX);
            pq.Push(new PQNode() { F = Math.Abs(_board.DestY - PosY) + Math.Abs(_board.DestX - PosX), G = 0, Y = PosY, X = PosX });
            parent[PosY,PosX] = new Pos(PosY,PosX); // 초기점이기에 자신을 넣음.

            while(pq.Count > 0)
            {
                // 제일 좋은 후보를 찾는다. 
                PQNode node = pq.Pop();
                // 동일한 좌표를 여러 경로를 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵 
                if (closed[node.Y, node.X])
                    continue;

                // 방문한다.
                closed[node.Y, node.X] = true;
                // 목적지 도착했으면 바로 종료 
                if (node.Y == _board.DestY && node.X == _board.DestX)
                    break;  

                // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다.
                for(int i = 0; i < deltaY.Length; i++)
                {
                    int nextY = node.Y + deltaY[i];
                    int nextX = node.X + deltaX[i];
                    
                    // 유효 범위를 벗어났으면 스킵
                    if (nextX < 0 || nextX >= _board.Size || nextY < 0 || nextY >= _board.Size) // 크기 벗어나면 넘겨
                        continue;
                    // 벽으로 막혀서 갈 수 없으면 스킵 
                    if (_board.Tile[nextY, nextX] == Board.TileType.Wall)
                        continue;
                    // 이미 방문한 곳이면 스킵 
                    if (closed[nextY, nextX])
                        continue;

                    // 비용 계산 
                    int g = node.G + cost[i];
                    int h = Math.Abs(_board.DestY - nextY) + Math.Abs(_board.DestX - nextX);
                    // 다른 경로에서 더 빠른 길 이미 찾았으면 스킵 
                    if (open[nextY, nextX] < g + h) // 계산 한것보다 지금이 더 빨라 
                        continue;

                    // 예약 진행 
                    open[nextY, nextX] = g + h;
                    pq.Push(new PQNode() { F = g + h, G = g, Y= nextY, X = nextX });
                    parent[nextY, nextX] = new Pos(node.Y, node.X);

                }
                
            }
            CalcPathFromParent(parent);
        }
    
        void BFS()
        {
            int[] deltaY = new int[] { -1, 0, 1, 0 };
            int[] deltaX = new int[] { 0, -1, 0, 1 };

            bool[,] found = new bool[_board.Size, _board.Size];
            Pos[,] parent = new Pos[_board.Size, _board.Size];

            Queue<Pos> q = new Queue<Pos>();
            q.Enqueue(new Pos(PosY, PosX));
            found[PosY, PosX] = true;
            parent[PosY, PosX] = new Pos(PosY, PosX);// 처음은 부모가 없으니 자신이 부모

            while (q.Count > 0)
            {
                Pos pos = q.Dequeue();
                int nowY = pos.Y;
                int nowX = pos.X;

                for (int i = 0; i < 4; i++)
                {
                    int nextY = nowY + deltaY[i];
                    int nextX = nowX + deltaX[i];

                    if (nextX < 0 || nextX >= _board.Size || nextY < 0 || nextY >= _board.Size) // 크기 벗어나면 넘겨
                        continue;
                    if (_board.Tile[nextY, nextX] == Board.TileType.Wall)
                        continue;
                    if (found[nextY, nextX])// 이미 발견한 길이면 넘겨 
                        continue;

                    q.Enqueue(new Pos(nextY, nextX));
                    found[nextY, nextX] = true;
                    parent[nextY, nextX] = new Pos(nowY, nowX);
                }
            }

            CalcPathFromParent(parent);
        }

        private void CalcPathFromParent(Pos[,] parent)
        {
            int y = _board.DestY;
            int x = _board.DestX;

            while (parent[y, x].Y != y || parent[y, x].X != x) // 부모가 도착점이면 더이상 안해도 되니까
            {
                _points.Add(new Pos(y, x));
                Pos pos = parent[y, x];
                y = pos.Y;
                x = pos.X;
            }
            _points.Add(new Pos(y, x));
            _points.Reverse();// 리스트를 뒤집어서 처음부터 시작
        }

        private void RightHand()
        {
            // 현재 바라보고 있는 방향을 기준으로, 좌표 변화를 나타낸다
            int[] frontY = new int[] { -1, 0, 1, 0 };
            int[] frontX = new int[] { 0, -1, 0, 1 };
            int[] rightY = new int[] { 0, -1, 0, 1 };
            int[] rightX = new int[] { 1, 0, -1, 0 };

            _points.Add(new Pos(PosY, PosX));
            // 목적지 도착하기 전에는 계속 실행
            while (PosY != _board.DestY || PosX != _board.DestX)
            {
                // 1. 현재 바라보는 방향을 기준으로 오른쪽으로 갈 수 있는지 확인.
                if (_board.Tile[PosY + rightY[_dir], PosX + rightX[_dir]] == Board.TileType.Empty)
                {
                    // 오른쪽 방향으로 90도 회전
                    _dir = (_dir - 1 + 4) % 4;
                    // 앞으로 한 보 전진.
                    PosY = PosY + frontY[_dir];
                    PosX = PosX + frontX[_dir];
                    _points.Add(new Pos(PosY, PosX));
                }
                // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
                else if (_board.Tile[PosY + frontY[_dir], PosX + frontX[_dir]] == Board.TileType.Empty)
                {
                    // 앞으로 한 보 전진.
                    PosY = PosY + frontY[_dir];
                    PosX = PosX + frontX[_dir];
                    _points.Add(new Pos(PosY, PosX));
                }
                else
                {
                    // 왼쪽 방향으로 90도 회전
                    _dir = (_dir + 1 + 4) % 4;
                }
            }
        }


        const int MOVE_TICK = 100;
		int _sumTick = 0;
		int _lastIndex = 0;
		public void Update(int deltaTick)
		{
			if (_lastIndex >= _points.Count)
            {
                _lastIndex = 0;
                _points.Clear();
                _board.Initialize(_board.Size, this);
                Initialize(1, 1, _board);
            }

			_sumTick += deltaTick;
			if (_sumTick >= MOVE_TICK)
			{
				_sumTick = 0;

				PosY = _points[_lastIndex].Y;
				PosX = _points[_lastIndex].X;
				_lastIndex++;
			}
		}
	}
}
