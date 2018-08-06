###############################
#Sanity Checks
###############################
import os
try:
	import load
except ImportError:
	print("The module load does not exist in this machine. Please install load")
	raise SystemExit("Error")
try:
	import theano
except ImportError:
	print("The module theano does not exist in this machine. Please install theano")
	raise SystemExit
try:
	import theano.tensor as T
except ImportError:
	print("The module theano.tensor does not exist in this machine. Please install theano.tensor")
	raise SystemExit
try:
	import math
except ImportError:
	print("The module math does not exist in this machine. Please install math")
	raise SystemExit
try:
	import chess, chess.pgn
except ImportError:
	print("The module chess does not exist in this machine. Please install chess")
	raise SystemExit
try:
	import heapq
except ImportError:
	print("The module heapq does not exist in this machine. Please install heapq")
	raise SystemExit
try:
	import time
except ImportError:
	print("The module time does not exist in this machine. Please install time")
	raise SystemExit
try:
	import re
except ImportError:
	print("The module re does not exist in this machine. Please install re")
	raise SystemExit
try:
	import string
except ImportError:
	print("The module string does not exist in this machine. Please install string")
	raise SystemExit
try:
	import numpy
except ImportError:
	print("The module numpy does not exist in this machine. Please install numpy")
	raise SystemExit
try:
	import sunfish
except ImportError:
	print("The module sunfish does not exist in this machine. Please install sunfish")
	raise SystemExit
try:
	import pickle
except ImportError:
	print("The module pickle does not exist in this machine. Please install pickle")
	raise SystemExit
try:
	import random
except ImportError:
	print("The module random does not exist in this machine. Please install random")
	raise SystemExit
try:
	import traceback
except ImportError:
	print("The module traceback does not exist in this machine. Please install traceback")
	raise SystemExit

###############################
#Script Start
###############################
	
def getModel(fn):
    #gets the model from the pickle file and turns it into a function
    f = open(fn, 'rb')
    Ws, bs = pickle.load(f)
    
    Ws_s, bs_s = load.get_parameters(Ws=Ws, bs=bs)
    x, p = load.get_model(Ws_s, bs_s)
    
    predict = theano.function(
        inputs=[x],
        outputs=p)

    return predict

strip_whitespace = re.compile(r"\s+")
translate_pieces = str.maketrans(".pnbrqkPNBRQK", "\x00" + "\x01\x02\x03\x04\x05\x06" + "\x08\x09\x0a\x0b\x0c\x0d")

def positionToArray(pos, flip):
    # Create a numpy array from board representation
    pos = strip_whitespace.sub('', pos.board)
	# Translate the board into the numerical representation
    pos = pos.translate(translate_pieces)
    m = numpy.fromstring(pos, dtype=numpy.int8)
    if flip:
        m = numpy.fliplr(m.reshape(8, 8)).reshape(64)
    return m

CHECKMATE_SCORE = 1e6

def negamax(pos, depth, alpha, beta, color, func):
    #The negamax algorithm for parsing through board position evaluations by the neural network
    moves = []
    X = []
    children_positions = []
    for move in pos.gen_moves():
        child_position = pos.move(move)
        moves.append(move)
        X.append(positionToArray(child_position, flip=(color==1)))
        children_positions.append(child_position)

    if len(X) == 0:
        return Exception('eh?')

    # Use model to predict scores
    scores = func(X)

    for i, child_position in enumerate(children_positions):
        if child_position.board.find('K') == -1:
            scores[i] = CHECKMATE_SCORE

    child_nodes = sorted(zip(scores, moves), reverse=True)

    best_value = float('-inf')
    best_move = None
    
    for score, move in child_nodes:
        if depth == 1 or score == CHECKMATE_SCORE:
            value = score
        else:
            child_position = pos.move(move)
            negamax_value, _ = negamax(child_position, depth-1, -beta, -alpha, -color, func)
            value = -negamax_value

        if value > best_value:
            best_value = value
            best_move = move

        if value > alpha:
            alpha = value

        if alpha > beta:
            break

    return best_value, best_move


def create_move(board, crdn):
    move = chess.Move.from_uci(crdn)
    if board.piece_at(move.from_square).piece_type == chess.PAWN:
        if int(move.to_square/8) in [0, 7]:
            move.promotion = chess.QUEEN
    return move


class Player(object):
    def move(self, gn_current):
        raise NotImplementedError()


class Computer(Player):
    def __init__(self, func, maxd=5):
        self._func = func
        self._pos = sunfish.Position(sunfish.initial, 0, (True,True), (True,True), 0, 0)
        self._maxd = maxd

    def move(self, gn_current):
        assert(gn_current.board().turn == True)

        if gn_current.move is not None:
            crdn = str(gn_current.move)
            move = (119 - sunfish.parse(crdn[0:2]), 119 - sunfish.parse(crdn[2:4]))
            self._pos = self._pos.move(move)

        alpha = float('-inf')
        beta = float('inf')

        depth = self._maxd
        t0 = time.time()
        best_value, best_move = negamax(self._pos, depth, alpha, beta, 1, self._func)
        crdn = sunfish.render(best_move[0]) + sunfish.render(best_move[1])
        print (depth, best_value, crdn, time.time() - t0)

        self._pos = self._pos.move(best_move)
        crdn = sunfish.render(best_move[0]) + sunfish.render(best_move[1])
        move = create_move(gn_current.board(), crdn)
        
        gn_new = chess.pgn.GameNode()
        gn_new.parent = gn_current
        gn_new.move = move


        return gn_new


class Human(Player):
    def move(self, gn_current):
        bb = gn_current.board()

        print (bb)

        def get_move(move_str):
            try:
                move = chess.Move.from_uci(move_str)
            except:
                print ('cant parse')
                return False
            if move not in bb.legal_moves:
                print ('not a legal move')
                return False
            else:
                return move

        while True:
            print ('your turn:')
            move = get_move(raw_input())
            if move:
                break

        gn_new = chess.pgn.GameNode()
        gn_new.parent = gn_current
        gn_new.move = move
        
        return gn_new


class Sunfish(Player):
    def __init__(self, secs=1):
        self._searcher = sunfish.Searcher()
        self._pos = sunfish.Position(sunfish.initial, 0, (True,True), (True,True), 0, 0)
        self._secs = secs

    def move(self, gn_current):
        import sunfish

        assert(gn_current.board().turn == False)

        # Apply last_move
        crdn = str(gn_current.move)
        move = (sunfish.parse(crdn[0:2]), sunfish.parse(crdn[2:4]))
        self._pos = self._pos.move(move)

        t0 = time.time()
        move, score = self._searcher.search(self._pos, self._secs)
        print (time.time() - t0, move, score)
        self._pos = self._pos.move(move)

        crdn = sunfish.render(119-move[0]) + sunfish.render(119 - move[1])
        move = create_move(gn_current.board(), crdn)
        
        gn_new = chess.pgn.GameNode()
        gn_new.parent = gn_current
        gn_new.move = move

        return gn_new

def game(func):
    gn_current = chess.pgn.Game()

    maxd = random.randint(1, 2)
    secs = random.random()

    print ('maxd %f secs %f' % (maxd, secs))

    player_a = Computer(func, maxd=maxd)
    player_b = Sunfish(secs=secs)

    times = {'A': 0.0, 'B': 0.0}
    
    while True:
        for side, player in [('A', player_a), ('B', player_b)]:
            t0 = time.time()
            try:
                gn_current = player.move(gn_current)
            except KeyboardInterrupt:
                return
            except:
                traceback.print_exc()
                return side + '-exception', times

            times[side] += time.time() - t0
            print ('=========== Player %s: %s' % (side, gn_current.move))
            s = str(gn_current.board())
            print (s)
            if gn_current.board().is_checkmate():
                return side, times
            elif gn_current.board().is_stalemate():
                return '-', times
            elif gn_current.board().can_claim_fifty_moves():
                return '-', times
            elif s.find('K') == -1 or s.find('k') == -1:
                return side, times
            
def play():
    func = getModel('model.pickle')
    while True:
        side, times = game(func)
        f = open('stats.txt', 'a')
        f.write('%s %f %f\n' % (side, times['A'], times['B']))
        f.close()

        
if __name__ == '__main__':
    play()