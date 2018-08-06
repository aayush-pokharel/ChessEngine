###############################
#Sanity Checks
###############################
try:
	import load
except ImportError:
	print("The module load does not exist in this machine. Please install load")
	raise SystemExit
try:
	import numpy
except ImportError:
	print("The module numpy does not exist in this machine. Please install numpy")
	raise SystemExit
try:
	import os
except ImportError:
	print("The module os does not exist in this machine. Please install os")
	raise SystemExit
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
	from theano.tensor.nnet import sigmoid
except ImportError:
	print("The module theano.tensor.nnet does not exist in this machine. Please install theano.tensor.nnet")
	raise SystemExit
try:
	from sklearn.model_selection import train_test_split
except ImportError:
	print("The module sklearn does not exist in this machine. Please install sklearn")
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
	import itertools
except ImportError:
	print("The module itertools does not exist in this machine. Please install itertools")
	raise SystemExit
try:
	import scipy.sparse
except ImportError:
	print("The module scipy does not exist in this machine. Please install scipy")
	raise SystemExit
try:
	import h5py
except ImportError:
	print("The module h5py does not exist in this machine. Please install h5py")
	raise SystemExit
try:
	import math
except ImportError:
	print("The module math does not exist in this machine. Please install math")
	raise SystemExit
try:
	import time
except ImportError:
	print("The module time does not exist in this machine. Please install time")
	raise SystemExit

###############################
#Script Start
###############################
	
#Mini batch size constant
MINIBATCH_SIZE = 2000

def floatX(x):
    return numpy.asarray(x, dtype=theano.config.floatX)

	
def nesterov(loss, all_params, learn_rate, momentum):
	updates = []
	all_grads = T.grad(loss, all_params)
	for param_i, grad_i in zip(all_params, all_grads):
		# generate a momentum parameter
		mparam_i = theano.shared(
			numpy.array(param_i.get_value()*0., dtype=theano.config.floatX))
		v = momentum * mparam_i - learn_rate * grad_i
		w = param_i + momentum * v - learn_rate * grad_i
		updates.append((param_i, w))
		updates.append((mparam_i, v))
	return updates
	

def load_data(dir='./trainingData'):
	#load data from the parsed hdf5 files
	for fn in os.listdir(dir):
		if not fn.endswith('.hdf5'):
			continue
		fn = os.path.join(dir, fn)
		try:
			yield h5py.File(fn, 'r')
		except:
			print ('could not read ', fn)

def get_data(series=['x', 'xr']):
	#Get data parameters from the hdf5 files 
	#and return the test and training set
	data = [[] for s in series]
	for f in load_data():
		try:
			for i, s in enumerate(series):
				data[i].append(f[s].value)
		except:
			raise
			print ('failed reading from', f)
	def stack(vectors):
		if len(vectors[0].shape) > 1:
			return numpy.vstack(vectors)
		else:
			return numpy.hstack(vectors)
	data = [stack(d) for d in data]
	print(data[0])
	#Set the testing data set ratio
	test_size = 10000.0 / len(data[0])
	print ("Splitting " + str(len(data[0])) + " entries into train/test set")
	data = train_test_split(*data, test_size=test_size)

	print (str(data[0].shape[0]) + " train set " + str(data[1].shape[0]) + " test set")
	return data

def get_training_model(Ws_s, bs_s, dropout=False, lambd=10.0, kappa=1.0):
	# Build a dual network, one for the real move, one for a fake random move
	# Train on a negative log likelihood of classifying the right move

	xc_s, xc_p = load.get_model(Ws_s, bs_s, dropout=dropout)
	xr_s, xr_p = load.get_model(Ws_s, bs_s, dropout=dropout)
	xp_s, xp_p = load.get_model(Ws_s, bs_s, dropout=dropout)


	cr_diff = xc_p - xr_p
	loss_a = -T.log(sigmoid(cr_diff)).mean()

	cp_diff = kappa * (xc_p + xp_p)
	loss_b = -T.log(sigmoid( cp_diff)).mean()
	loss_c = -T.log(sigmoid(-cp_diff)).mean()

	# Add regularization terms
	reg = 0
	for x in Ws_s + bs_s:
		reg += lambd * (x ** 2).mean()

	loss = loss_a + loss_b + loss_c
	return xc_s, xr_s, xp_s, loss, reg, loss_a, loss_b, loss_c
	
def get_function(Ws_s, bs_s, dropout=False, update=False):
	#This method retreives the model to be trained on
	
	xc_s, xr_s, xp_s, loss_f, reg_f, loss_a_f, loss_b_f, loss_c_f = get_training_model(Ws_s, bs_s, dropout=dropout)
	obj_f = loss_f + reg_f

	learning_rate = T.scalar(dtype=theano.config.floatX)

	momentum = floatX(0.9)

	if update:
		updates = nesterov(obj_f, Ws_s + bs_s, learning_rate, momentum)
	else:
		updates = []
	print(learning_rate)
	print ('compiling function')
	f = theano.function(
		inputs=[xc_s, xr_s, xp_s, learning_rate],
		outputs=[loss_f, reg_f, loss_a_f, loss_b_f, loss_c_f],
		updates=updates,
		on_unused_input='warn')

	return f

def train():
	#Get test and train data for each paramter of
	#parent, observed and random moves
	Xc_train, Xc_test, Xr_train, Xr_test, Xp_train, Xp_test = get_data(['x', 'xr', 'xp'])
	
	#Print the board representation to be passed in
	for board in [Xc_train[0], Xp_train[0]]:
		for row in range(8):
			print (' '.join('%2d' % x for x in board[(row*8):((row+1)*8)]))
		print("\n")
	
	n_in = 12 * 64
	
	#Get the parmeters for training
	Ws_s, bs_s = load.get_parameters(n_in=n_in, n_hidden_units=[2048] * 3)
	minibatch_size = min(MINIBATCH_SIZE, Xc_train.shape[0])
	
	#Get the training and test sets
	train = get_function(Ws_s, bs_s, update=True, dropout=False)
	test = get_function(Ws_s, bs_s, update=False, dropout=False)

	#Set initail values for
	#test loss and the base learning rate and number of iterations
	best_test_loss = float('inf')
	base_learning_rate = 0.03
	t0 = time.time()

	i = 0
	#Train loop
	while True:
		i += 1
		#calculate the learning rate
		learning_rate = floatX(base_learning_rate * math.exp(-(time.time() - t0) / 86400))
		#calculate the training loss
		minibatch_index = random.randint(0, int(Xc_train.shape[0] / minibatch_size) - 1)
		lo, hi = minibatch_index * minibatch_size, (minibatch_index + 1) * minibatch_size
		loss, reg, loss_a, loss_b, loss_c = train(Xc_train[lo:hi], Xr_train[lo:hi], Xp_train[lo:hi], learning_rate)
		zs = [loss, loss_a, loss_b, loss_c, reg]
		#Print the learning rate and current loss
		print ("iteration %6d learning rate %12.9f: %s" % (i, learning_rate, '\t'.join(["%12.9f" % z for z in zs])))
		#every 200 iterations check if the test loss is better than the best loss recorded
		if i % 200 == 0:
			test_loss, test_reg, _, _, _ = test(Xc_test, Xr_test, Xp_test, learning_rate)
			#Print he test loss
			print ("test loss %12.9f" % test_loss)
			#if test loss is better than the best loss then dump model parameters to model.pickle
			if test_loss < best_test_loss:
				print ("new record!")
				best_test_loss = test_loss

				print ("dumping pickled model")
				f = open('model.pickle', 'wb')
				def values(zs):
					return [z.get_value(borrow=True) for z in zs]
				pickle.dump((values(Ws_s), values(bs_s)), f)
				f.close()

	

if __name__ == '__main__':
	train()