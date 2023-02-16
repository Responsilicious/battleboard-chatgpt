<?php

// Define the Cell class
class Cell {
    private $state = 0;
    private $ship = null;

    public function setState($state) {
        $this->state = $state;
    }

    public function getState() {
        return $this->state;
    }

    public function setShip($ship) {
        $this->ship = $ship;
    }

    public function getShip() {
        return $this->ship;
    }
}

// Define the Ship class
class Ship {
    private $size;
    private $cells = [];

    public function __construct($size) {
        $this->size = $size;
    }

    public function addCell($cell) {
        $this->cells[] = $cell;
    }

    public function getSize() {
        return $this->size;
    }

    public function isSunk() {
        foreach ($this->cells as $cell) {
            if ($cell->getState() != 2) {
                return false;
            }
        }
        return true;
    }
}

// Define the Board class
class Board
{
    private $cells = [];
    private $ships = [];

    public function __construct()
    {
        for ($i = 0; $i < 10; $i++) {
            $this->cells[] = [];
            for ($j = 0; $j < 10; $j++) {
                $this->cells[$i][] = new Cell();
            }
        }

        $this->placeShips();
    }

    private function placeShips()
    {
        $ships = [5, 4, 3, 3, 2];

        foreach ($ships as $ship) {
            $ship_placed = false;

            while (!$ship_placed) {
                $x = rand(0, 9);
                $y = rand(0, 9);

                $orientation = rand(0, 1);

                if ($orientation == 0) {
                    // horizontal placement
                    $valid = true;
                    for ($i = 0; $i < $ship; $i++) {
                        if ($this->cells[$y][$x + $i]->getState() != 0) {
                            $valid = false;
                            break;
                        }
                    }
                    if ($valid) {
                        $new_ship = new Ship($ship);
                        for ($i = 0; $i < $ship; $i++) {
                            $cell = $this->cells[$y][$x + $i];
                            $cell->setState(1);
                            $cell->setShip($new_ship);
                            $new_ship->addCell($cell);
                        }
                        $this->ships[] = $new_ship;
                        $ship_placed = true;
                    }
                } else {
                    // vertical placement
                    $valid = true;
                    for ($i = 0; $i < $ship; $i++) {
                        if ($this->cells[$y + $i][$x]->getState() != 0) {
                            $valid = false;
                            break;
                        }
                    }
                    if ($valid) {
                        $new_ship = new Ship($ship);
                        for ($i = 0; $i < $ship; $i++) {
                            $cell = $this->cells[$y + $i][$x];
                            $cell->setState(1);
                            $cell->setShip($new_ship);
                            $new_ship->addCell($cell);
                        }
                        $this->ships[] = $new_ship;
                        $ship_placed = true;
                    }
                }
            }
        }
    }

    public function getCellState($x, $y)
    {
        return $this->cells[$y][$x]->getState();
    }

    public function shoot($x, $y)
    {
        $cell = $this->cells[$y][$x];
        if ($cell->getState() == 0 || $cell->getState() == 2) {
            $cell->setState(3);
            return false;
        } else {
            $cell->setState(2);
            $ship = $cell->getShip();
            if ($ship->isSunk()) {
                return 'sunk';
            } else {
                return 'hit';
            }
        }
    }
}
