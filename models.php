<?php

class Model_Skill extends RedBean_SimpleModel
{
	public function usage_count()
	{
		return $this->bean->withCondition('is_published')->countShared('portfolio');
	}
}