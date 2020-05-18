# Astar-search-on-a-2D-Array
This was my first attempt on trying to solve a 5x5 tile game using A* search algorithm.
This failed to solve the puzzle in time but the algorithm still works. In order of attempts,
this is what I tried to do:

1) A* search normally- this took way too long as there are 25! possible combinations
2) Lock numbers when they reach the right position, starting with the top left. If 1 is in the right
position, lock it. Then we can lock 2 and 6 once they reach the right position, and so on diagonally.
This didn't work because it sometimes creates a gap on the far right that cannot be solved.
3) Don't lock numbers in the second to last row and column unless the adjacent last row or column 
is also in the right position.
4) Add 'weights' to the manhattan forumla for items in the top left to prioritize moving them first.

Despite these changes, the algorithm would still take too long. For my next approach, I will be starting over,
and instead making the program seek out the numbers directly to move them.
