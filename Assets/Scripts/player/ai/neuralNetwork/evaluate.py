import load
import pickle
import theano
import theano.tensor as T
import math
import chess, chess.pgn
import heapq
import time
import re
import string
import numpy
import random
import traceback
import sunfish

class Evaluator():
	def __init__():
		func = get_model_from_pickle('model.pickle')
		
		
	def get_model_from_pickle(fn):
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
		
	def pos2array(pos, flip):
		pos = strip_whitespace.sub('', pos) # should be 64 characters now
		print("Strip white space")
		print(pos)
		pos = pos.translate(translate_pieces)
		print("Translated pieces")
		print(pos)
		m = numpy.fromstring(pos, dtype=numpy.int8)
		if flip:
			m = numpy.fliplr(m.reshape(8, 8)).reshape(64)
		return m

	def evaluate(boards):
		color = 1

		# boardPosition = sunfish.Position(sunfish.initial, 0, (True,True), (True,True), 0, 0)
		# positionArray = pos2array(boardPosition.board, flip=(color==1))
		# print(boardPosition.board)
		# print(positionArray)
		#boardPosition = "  r  .  b  q  k  b  n  r\n  p  p  p  p  .  p  p  p\n  .  .  n  .  p  .  .  .\n  .  .  .  .  .  .  .  .\n  .  .  .  P  .  .  .  .\n  .  .  .  .  .  .  .  N\n  P  P  P  .  P  P  P  P\n  R  N  B  Q  K  B  .  R\n"
		boardpositions = []
		for pos in boards:
			boardPosition.append(pos.replace(" ", ""))
		positionArray = pos2array(boardPosition, flip=(color==1))
		print(boardPosition)
		print(positionArray)
		x = []
		x.append(positionArray)
		score = func(x)
		print(score[0])
		