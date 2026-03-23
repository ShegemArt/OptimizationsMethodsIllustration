syms x y 
f = 100 * (y - x * x) * (y - x * x) + 5 * (1 - x) * (1 - x)
dfdx = diff(f, x);
dfdy = diff(f, y);

grad = [dfdx;dfdy];

dfdxdx = diff(dfdx, x);
dfdxdy = diff(dfdx, y);
dfdydx = diff(dfdy, x);
dfdydy = diff(dfdy, y);

Gessian = [dfdxdx, dfdxdy; dfdydx, dfdydy];


x_curr = [x; y];

x_new = x_curr - inv(Gessian) * grad;