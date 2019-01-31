import sys
import random
from datetime import datetime


def roll_check(modifier, target):

    random.seed(datetime.now())
    roll = 0

    for i in range(1, 3):
        roll = roll + random.randrange(1, 6)

    if roll == 3 or roll == 4 or roll == 5:
        return "Critical Failure"
    elif roll == 16 or roll == 17 or roll == 18:
        return "Critical Success"
    else:
        roll = roll + int(modifier)

    return "Success" if roll >= int(target) else "Failure"


print(roll_check(sys.argv[1], sys.argv[2]))
